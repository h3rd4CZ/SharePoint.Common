# Common library to build full-trust code on-premises SharePoint applications

Význam jednotlivých vrstev:

| Vrstva  |Typ vrstvy | Význam vrstvy|
| ------------- | ------------- | ------------- |
| Common | Obecná akstrakce| Obsahuje základní  objekty / služby, systémové třídy pro základní objekty, POCO objekty atd...  |
| DataAccess  | Datová abstrakce| Obsahuje abstraktní objekty přístupu k datům DTO objekty, fasády, factory. |
| DataAccess implementation  |Datová implementace| Vrstva implementace datového přístupu. Těchto vrstev je v rozsáhlejších systémech typicky více. Nejčastější vrstvy v klientských řešeních jsou SharePoint a Sql|
| Implementation  |Obecná implementace | Obsahuje v podstatě implementaci doménové / business logiky pro konkrétní komponentu |

------------
Řešení by měla dodržovat následující pravidla objektově orientovaného přístupu, jsou to především:
 - SOLID, především open-close a dependency inversion princip
 - Kódovat proti abstrakcím, nikoliv implementacím
 - Loose coupling
 - Systémové ošetřování vstupů metod a výsledků událostí (kroků) při vykonávání těchto metod
 - Maximalizace absence konstruktu **new** ve zdrojovém kódu
 - No spagheti code
- DRY
- IoC
- DI
-  Organizace kódu dle DDD a komponentového rozdělení dle popisu výše

## Knihovna
Samotná knihovna je distribuována jako sada NPM balíčků rozdělených do jednotlivých komponentových vrstev. V rámci CI je každá nová verze publikována do interního NPM feedu realizovaného pomocí Azure Artifacts.

[RhDev common library Azure Artifacts feed](https://dev.azure.com/RhDev-BP/RhDev/_packaging?_a=feed&feed=dxnet "RhDev common library Azure Artifacts feed")


![](https://storageaccountscdlpa8ad.blob.core.windows.net/imgcontainer/NugetCommonLib.png)

#### Minimální požadavky

| Platforma  | Minimální verze   |
| ------------ | ------------ |
| SharePoint  |  2016  |
| .net Framework  | 4.7.2   |
| .net CLR  | 4.0   |
| Nuget  | XXX (musí podporovat PackageReference z *.csproj), [Nuget PackageReference](https://docs.microsoft.com/cs-cz/nuget/consume-packages/package-references-in-project-files)   |
| Visual studio  | 2017   |

#### Kontinuální integrace
Knihovna je napojena v rámci Azure Devops na kontinuální integraci. Každý nový přírůstek generuje novu sadu knihoven do interního Nuget feedu, kterou poté využívají jedntlivá klientská řešení založená na společné šabloně.

------------

Pipeline buildu obsahuje posloupnost následujících akcí:

| Fáze  |Popis   |
| ------------ | ------------ |
| 0. Checkout zdrojových kódů  | Zpracování zdrojových kódů řešení   |
| 1. NuGetToolInstaller  | Instalace balíčkovacího systému Nuget   |
| 2. NuGetCommand   | Instalace veškerých Nuget balíčků dle definice řešení |
| 3. PowerShell   |  Zapsání Nuget verze balíčků knihoven dle čísla buildu. Číslo buildu je definováno jako : ***1.$(Year:yy).$(DayOfYear).$(BuildID)***  Major verzi knihovny je v budoucnu možné posunout ručně na vyšší verzi. |
| 4. Assembly-Info-NetFramework|  Zapsání verze buildu do *AssemblyInfo.cs*  do jednotlivých knihoven. Zapisuje se pouze File version, Assembly verze je nastavena defaultně na verzi **1.0.0.0**  |
| 5. Build   | Build všech projektů řešení v dané build konfiguraci |
| 6. Test   | Provedení definovaných testů. V rámci řešení je definováno několik testů rozdělěných do skupin. Code coverage je na cca 11%. V budoucnu je nutné zvýšit. Testovací třídy se nachází v assembly **RhDev.SharePoint.Common.Test.dll** |
| 7. CopyFiles   | Kopírování Nuget balíčků jako výsledku buildu do *artifactstagingdirectory* |
| 8. NuGetCommand - push   | Vypublikování hotových NUGET balíčků do interního feedu |

[Definice pipeline v Azure devops pipeline](https://dev.azure.com/RhDev-BP/RhDev/_apps/hub/ms.vss-build-web.ci-designer-hub?pipelineId=8&branch=master "Definice pipeline v Azure devops pipeline")

#### IoC

Inversion of control umožňuje psát kód vyhovující principům volných vazeb mezi komponentami, testovatelnosti, dobré organizace a rozšiřitelnosti komponent daného řešení. 
Řešení využívá open source projektu **StructureMap**-
Jednotlivá klientská řešení používají rozhraní IoC manageru knihovny pro sestavení vlastního IoC kontejneru. Řešení obsahuje dvou-úrovňovou registraci komponent pomocí oddělěných kontejnerů zvlášť pro frontend služby a zvlášť pro backend služby.

| Kontejner  | Způsob použití   |
| ------------ | ------------ |
| Frontend  | Aplikační stránky, webové části, webové služby, obecně pro kód běžící v aplikačním poolu webových aplikací   |
| Backend  | Časové úlohy, windows schedulery, konzolové aplikace, obecně platí pro veškerý kód mimo stadardní frontendový SPContext   |


Registrace komponent se provádí v implementačních vrstvách **DataAccess.** a **Implementation**.
Každá vrstva může obsahovat registraci do Frontend kontejneru i do backend kontejneru.
Registrace v backend kontejneru (pro backend kontejner) přepisuje registrace ve Frontend kontejneru.
Při požadavku o sestavení kontejneru je nutné odeslat definici na sestavení kontejneru. Tuto definici je možné setavit pomocí vnitřního builderu:

Příklad požadavku na sestavení kontejneru:

> CompositionDefinition.cs

```csharp
using RhDev.SharePoint.Common.Composition.Factory.Builder;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Composition
{
    public static class CompositionDefinition
    {
        public static ContainerRegistrationDefinition GetDefinition() => ContainerRegistrationDefinitionBuilder
                .Get("RhDev.Customer.Solution")
                .WithComponents(new List<ContainerRegistrationComponentDefinition>
                {
                    ContainerRegistrationDefinitionComponentBuilder.Get("Common")
                        .WithLayers(
                            new List<ContainerRegistrationLayerDefinition>{
                                ContainerRegistrationDefinitionLayerBuilder.GetNativeDataAccessSharePointLayer()
                                .WithFrontendAndBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion().Build(),
                                 ContainerRegistrationDefinitionLayerBuilder.Get("DataAccess.Sql")
                                .WithFrontendRegistrations()
                               .WithDefaultRhDevPKTAndVersion().Build(),
                                ContainerRegistrationDefinitionLayerBuilder.GetNativeImplementationLayer()
                                .WithBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion().Build(),
                        }).Build()
                }).Build();
    }
}

```

Registrace výše popisuje požadavek pro sestavení kontejneru, který obsahuje jednu komponentu **Common** a 3 registrační vrstvy
1. GetNativeDataAccessSharePointLayer() - ( **DataAccess.SharePoint** )
2. **DataAccess.Sql**
3. GetNativeImplementationLayer - ( **Impl**)

Každá z těchto vrstev reprezentuje class library, ve které se musí nacházet jednotlivé registrační třídy.

- **WithFrontendAndBackendRegistrations()** určuje že daná vrstva musí obsahovat registrace pro frontend i pro backend
- **WithFrontendRegistrations()** určuje že daná vrstva musí obsahovat registrace pro frontend
- **WithBackendRegistrations** určuje že daná vrstva musí obsahovat registrace pro backend

Definice registrace služeb se provádí pro Frontend vytvořením třídy **DefaultCompositionConfiguration.cs** (název rozhoduje case - sensitive) v class library reprezentující příslušnou vrstvu. Příklad registrace může vypadat:

> DefaultCompositionConfiguration.cs

```csharp
public class DefaultCompositionConfiguration : ConventionConfigurationBase
    {
        public DefaultCompositionConfiguration(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {

        }

        public override void Apply()
        {
            base.Apply();
                        
            For(typeof(ICache<>)).Singleton();
            For(typeof(ICache<>)).Use(typeof(DictionaryCache<>));

            For(typeof(IConfigurationManager<>)).Use(typeof(ConfigurationManager<>));

            For<IConcurrentDataAccessRepository>().Singleton();
        }
    }
```

V metodě *Apply()* se provádí jednotlivé registrace. Definice registrace služeb pro backend se provádí analogicky ve třídě **TimerJobCompositionConfigurationOverrides**.

##### Autoregistrace
V rámci registrace komponent je možné služby definovat jako auto-registrační. Takové služby jsou v kontejneru registrovány automaticky proti rozhraní, které implementují. Auto registrace se provádí implementací rozhraní *IAutoRegisteredService*:

> ICentralClockProvider.cs

```csharp
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common
{
    public interface ICentralClockProvider : IAutoRegisteredService
    {
        CentralClock Now();
    }
}

```

##### Assembly version a public key token
Strong name knihoven musí obsahovat následující záznamy:

| Verze  | Publick key token   |
| ------------ | ------------ |
|1.0.0.0   | 78afb44363f8be41   |

Verze všech knihoven musí být neměnná a nastavená na verzi **1.0.0.0**. Platí pro všechna RhDev BP řešení, u kterých dochází k IoC registracím pomocí kontejneru. Public key token je generován veřejným klíčem, kterým jsou podepisovány jednolivé RhDev common library. Tento klíč je přiložen v řešení vytvářeném ze šablony, vývojář řešení se nemusí o nic starat. Tyto defaultní parametry je možné v definici registrace zapsat pomocí builderu zmíněného výše:
```csharp
WithDefaultRhDevPKTAndVersion()
```
Metoda builderu výše zajistí definici s defaultními parametry verze a PKT.

##### Název class library
Každá class library reprezentující příslušnou vrstvu v dané komponentě by měla dodržovat následující jmennou konvenci:

```
RhDev.<Zakaznik>.<Reseni>.<Vrstva>
```

##### Implementace kontejneru v klientských řešeních
Každé řešení založené na společné šabloně obsahuje již předdefinovaný kontejner s registrací jedné defaultní komponenty:
```csharp
public static class IoC
    {
        private static readonly object SyncRoot = new object();

        private static IApplicationContainerSetup containerSetup;

        public static IApplicationContainerSetup Get
        {
            get
            {
                lock (SyncRoot)
                {
                    return containerSetup ?? (containerSetup = ApplicationContainerFactory.Create(CompositionDefinition.GetDefinition()));
                }
            }
        }
    }
```
Získání služby z backedn kontejneru může vypadat následovně:
```csharp
GlobalConfiguration = IoC.Get.Backend.GetInstance<Objects.GlobalConfiguration>();
```
Analogicky frontend kontejner:
```csharp
FarmConfiguration = IoC.Get.Frontend.GetInstance<FarmConfiguration>();
```
Build konkrétního objektu (this) z frontend kontejneru:
```csharp
IoC.Get.Frontend.BuildUp(this);
```
Při build konkrétních objektů (sestavení stromu závislostí všech komponent v daném objektu) pomocí posledního konstruktu je nutné aby všechny požadované služby byly implementovány jako veřejné vlastnosti daného objektu s getterem i setterem:
```csharp
public partial class Test : LayoutsPageBase
    {
        public ISecurityContext SecurityContext { get; set; }

        public ITraceLogger TraceLogger { get; set; }

        public IApplicationLogManager ApplicationLogManager { get; set; }

        public IDataStoreAcessRepositoryFactory DataStoreAcessRepositoryFactory { get; set; }
```

#### Komponenty
Knihovna poskytuje jednotné rozhraní pro společné služby, které se v jednotlivých zákaznických řešení vyskytují.
Všechny služby ze společné knihovny jsou v rámci IoC registrovány automaticky při sestavování kontejneru klientských řešení.

##### Logování
###### ULS
Knihovna obsahuje vlastní službu pro logování do systémového logu. Klientská řešení mohou používat systémový trace logger implementovaný ve společné knihovně. 

```csharp
public ITraceLogger TraceLogger { get; set; }

TraceLogger.Write("Test message");
```
Podpora pro logování do Windows event logu:
```csharp
TraceLogger.Event(Common.Setup.Tracing.TraceCategories.Service, "Message");
```
Společný trace logger obsahuje několik předdefinovaných kategorií včetně úrovní pro trace a event log.

| Kategorie  | Event severity   | Trace severity   | Co by měla daná kategorie logovat   |
| ------------ | ------------ | ------------ | ------------ |
| General  |Medium   | Information   |  Logování obecných komponent  |
| FrontEnd   | Medium   | Information   | Logování aplikačních stránek, webových částí, ...   |
| Common   |Medium   | Information   | Logování společných - nespecifických komponent, které nelze zařadit do konkrétní kategorie   |
| Security  | High   | Warning   | Práce s uživateli, skupinami, oprávněními   |
| WebService  | Monitorable   |  Verbose | Rozhraní webových služeb  |
| Integration  | Medium   |  Information | Integrace s externími systémy  |

Příslušnou kategorii logování je možné zvolit pomocí parametru metody, pokud není specifikováno je zvolena defaultní kategorie **General**. Pokud je zvolena neexistující kategorie je vyhozena vyjímka.

------------

Při definici vlastního trace loggeru je nutné definovat novou třídu trace loggeru dědící třídu *SharePointTraceLogger*. A implementovat vlastnost *GetConfiguration*.
```csharp
public class SolutionTraceLogger : SharePointTraceLogger
    {
        public override DiagnosticsServiceConfiguration GetConfiguration =>
										TraceConfiguration.GetDiagnosticsServiceConfiguration;
    }
```
Tuto třídu je nutné v rámci IoC zaregistrovat jako novou službu trace loggeru:
> DefaultCompositionConfiguration.cs

```csharp
    public class DefaultCompositionConfiguration : ConventionConfigurationBase
    {
        public DefaultCompositionConfiguration(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {

        }

        public override void Apply()
        {
            base.Apply();

            For<ITraceLogger>().Use<SolutionTraceLogger>();
        }
    }
```
Nový trace logger je nutné zaregistrovat a nainstalovat jako novou diagnostics service při aktivaci farm featury:
```csharp
   public class SolutionTracingInstallation : FeatureInstallation<SPWebService>
    {
        private readonly ITraceLogger traceLogger;

        public SolutionTracingInstallation(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;
        }

        protected override void DoExecuteInstallation(SPWebService scope)
        {
            traceLogger.Register();
        }

        protected override void DoExecuteUninstallation(SPWebService scope)
        {
            traceLogger.Unregister();
        }
    }
```
Použití trace loggeru je poté totožné. Druhou možností je logování pomocí bázové třídy *ServiceBase* a přepsání vlastnosti *TraceCategory*

```csharp
public class MySuperLoggableService : ServiceBase
{
	protected override TraceCategory TraceCategory => TraceCategories.Common;
	
	public void Foo()
	{
			WriteTrace("Cool trace message here");
	}
}
```

###### Aplikační log
Logování probíhá do aplikačního LOGu v podobě SharePoint seznamu na konkrétním webu. Na příslušném webu je nutné zajistit aktivaci featury ze společné knihovny. [viz WSP řešení a instalace společné knihovny na farmě SharePoint](#wsp-řešení-a-instalace-společné-knihovny-na-farmě-sharepoint)

```csharp
public IApplicationLogManager ApplicationLogManager { get; set; }
```
```csharp
ApplicationLogManager
.WriteLog("Application log message", "Page", ApplicationLogLevel.Information);
```
Aplikační log je strukturován následovně:
- Rok
  - Měsíc
    - Den
      - Záznam LOGu

Každý záznam LOGu obsahuje stringový parametr **msg**, zdroj **source** a úroveň **level**

##### Konfigurace
Knihovna obsahuje dva zdroje správy konfigurace.
###### Konfigurace na úrovni webu
Tato konfigurace je uložena prostřednictvím seznamu na příslušném webu aplikace. Každý záznam konfigurace obsahuje modul klíč a hodnotu. Jednotlivé záznamy konfigurace jsou agregovány dle modulů.

![](https://storageaccountscdlpa8ad.blob.core.windows.net/imgcontainer/AppConfiguration.PNG)

Každý modul konfigurace reprezentuje jedna bekendová třída a zajišťuje přístup k jednotlivým konfiguračním parametrům daného modulu:

```csharp
    public class GlobalConfiguration : ConfigurationObject
    {
	    protected const string MODULE_NAME = "ModuleX";
        private ConfigurationKey FooKey = new ConfigurationKey(MODULE_NAME, "Foo");
        
        public string Foo
        {
            get { return DataSource.GetValue(FooKey, "FooDefault").AsString; }
            set { DataSource.SetValue(FooKey, value); }
        }
		
        public GlobalConfiguration(IConfigurationDataSource dataSource) : base(dataSource)
        {
        }
    }
```
###### Farm konfigurace
Farm konfigurace aplikace je implementována prostřednictvím farm properties dané farmy. 
Do této konfigurace se ukládají například URL aplikací jednotlivých řešení. Knihovna obsahuje společný objekt *FarmConfiguration* vytvořený pro tento účel. Aby došlo k oddělění vlastností jednotlivých řešení každé klientské řešení musí implementovat vlastní třídu dědící *FarmPropertiesDataSource* a zaregistrovat v kontejneru proti *IConfigurationDataSource* farmové konfigurace. 

```csharp
public class SolutionFarmPropertiesDataSource : FarmPropertiesDataSource
    {
        protected override string FarmConfigPrefix => Const.SOLUTION_DISPLAY;

        public SolutionFarmPropertiesDataSource(ITraceLogger traceLogger) : base(traceLogger)
        {
        }
    }
```
Oddělení vlastností jednotlivých řešení zajišťuje vlastnost *FarmConfigPrefix*.

V kontejneru je nutné zaregistrovat:
```csharp
For<FarmConfiguration>().Use<FarmConfiguration>().Ctor<IConfigurationDataSource>().Is<SolutionFarmPropertiesDataSource>();
```
Řešení vytvořená pomocí společné šablony tyto registrace mají již předdefinované.
###### Implementace vlastního providera
Je možné implementovat také vlastní provider pro ukládání konfigurace. Stačí vytvořit konfigurační objekt děděním bázové třídy *ConfigurationObject* a v registraci kontejneru zaregistrovat nový konfigurační data source pro tento objekt:

```csharp
For<MyCustomConfiguration>().Use<MyCustomConfiguration>().Ctor<IConfigurationDataSource>().Is<MySpecialConfigurationDataSourceImplementation>();
```
###### Kešování konfigurace
Ve frontend kontejneru je defaultně z důvodu výkonnosti každá čtecí operace konfigurace kešována. Invalidace kešovaných záznamů probíhá při recyklaci procesu, ve kterém běží výkoný kód. Pro backend kontejner platí opak, veškerá čtecí operace jsou z pohledu cache průchozí a veškeré konfigurační parametry se načítají přímo z datových zdrojů.
Strategie kešování se řídí registrací typu *IConfigurationCacheStrategy*. 
Pro aktivaci kešovací strategie je nutné v daném kontejneru registrovat:
```csharp
For<IConfigurationCacheStrategy>().Use<WithCacheConfigurationCacheStrategy>();
```

Pro deaktovaci:
```csharp
For<IConfigurationCacheStrategy>().Use<WithoutCacheConfigurationCacheStrategy>();
```

##### Guarding
Knihovna obsahuje statickou třídu *Guard* poskytující API pro běžně se vykytující požadavky na ošetřování kódu.
```csharp
public override void SetValue(ConfigurationKey key, object value)
        {
            Guard.NotNull(key, nameof(key));
	
```
```csharp
Guard.StringNotNullOrWhiteSpace(area, nameof(area), "No default RhDev Logging service was registered. Please register this service first");

            Guard.CollectionNotNullAndNotEmpty(categories, nameof(categories), "No categories for default RhDev Logging service was registered.");
			
			Guard.CollectionNotNullAndNotEmpty(
                defaultArea.Categories.ToList(),
                nameof(defaultArea), "Default area categories are empty, please specify default categories when register the diagnostics service");
```
Ošetřování by se mělo používat minimálně u vstupních parametrů metod.

##### Notifikace
Knihovna obsahuje jednoduchý notifikační provider pro odesílání emailových oznámení prostřednictvím standardního .net emailového klienta (*SmtpClient*) nebo SharePoint klienta (*SPUtility.SendEmail*). Defaultně je použit .net emailový klient.

```csharp
var sender = container.Frontend.GetInstance<INotificationSender>();
var tl = container.Frontend.GetInstance<ITraceLogger>();
var uip = container.Frontend.GetInstance<IUserInfoProvider>();
var ui = uip.GetUserInfo(SectionDesignation.FromString("http://appurl"), @"i:0#.w|login");
var clock = CentralClock.FillFromDateTime(DateTime.Now);
var ma = new MailQueueItemAttachment()
            {
                Data = new byte[] { 1, 2, 3 },
                Name = "File.txt"
			};

            sender.SendNotifications(SectionDesignation.FromString("http://appurl"),
                new List<Notifications.Notification> {
                    new Notifications.Notification(
                        ui,
                        clock,
                        "<b>TEST MAIL content</b>", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
                        subject:"System integration subject",
                        attachments: new List<MailQueueItemAttachment>{ ma })
                }, 
                notifySuccessLogger: (n, m) => tl.Write($"Successfully sent message : {m.Subject}"), 
                notifyFailedLogger : (m , e) => tl.Write($"Failed sent message : {m.Subject} : {e}"));
```

##### Ribbon
Ribbon je možné implementovat poděděním abstraktní třídy *RibbonBase* Příklad defaultního Ribbonu v knihovně:
```csharp
    public class DefaultRibbon : RibbonBase
    {
        private const string CANCEL_ACTION = "Cancel";

        private string Url;
        private readonly bool _isSaveDisabled;

        public event EventHandler Cancel;
        public event EventHandler Save;

        public DefaultRibbon(Page page, string url, bool isSaveDisabled)
            : base(page)
        {
            ButtonLocalization.AddLocalization(CANCEL_ACTION, "Zavřít");
            Url = url;
            _isSaveDisabled = isSaveDisabled;
        }

        protected override TabDefinition GetTabDefinition()
        {
            var groups = new List<GroupDefinition>();

            var ribbonGroup = GetRibbonGroup();
            if (ribbonGroup != null && ribbonGroup.Controls.Any())
                groups.Add(ribbonGroup);

            var documentDetailTab = new TabDefinition
            {
                Id = "Action",
                Title = "Akce",
                Groups = groups.ToArray()
            };

            return documentDetailTab;
        }

        private GroupDefinition GetRibbonGroup()
        {
            var buttons = new List<ControlDefinition>();

            if(!_isSaveDisabled)
                AddButton(buttons, SAVE_ACTION, new Point(15, 1), ShowAlways, GetPostBackActionJavaScript(SAVE_ACTION));
            AddButton(buttons, CANCEL_ACTION, new Point(15, 15), ShowAlways, GetScriptForCloseButton(Url));

            return CreateGroup("Actions", "Akce", buttons);
        }
        
        protected override void ProcessPostBack(string eventTarget)
        {
            RaiseEventIfActionPostBack(eventTarget, CANCEL_ACTION, Cancel);
            RaiseEventIfActionPostBack(eventTarget, SAVE_ACTION, Save);
        }
    }
}

```
Instanci třídy daného Ribbonu je nutné do stránky vložit pomocí bázové metody *AddToPage*:
```csharp
defaultRibbon.AddToPage();
```
Postback pro jednotlivé akce (tlačítka) se registruje pomocí delegátů ve třídě implementace ribbonu.
```csharp
defaultRibbon.Cancel += defaultRibbon_Cancel
defaultRibbon.Save += defaultRibbon_Save

```

##### Cache
Knihovna poskytuje podporu pro rychlou generickou in-memory cache. Veřejné rozhraní vypadá následovně:
```csharp
   public interface ICache<TValue> : IService where TValue : class 
    {
        IList<KeyValuePair<CacheKey, TValue>> GetCacheContent { get; }
        void AddValue(CacheKey key, TValue value);
        TValue GetValue(CacheKey key);
        TValue GetOrFetchValue(CacheKey key, Func<TValue> valueProvider);
        void Clear();
        void InvalidateCacheItem(CacheKey key);
    }
```
Ve společné knihovně jsou vytvořeny dvě implementace. **DictionaryCache** a **ExpirationDictionaryCache** . Druhá varianta umožňuje nastavit expiraci konfiguračním parametrem **ExpirationCacheConfiguration**. Parametr je nutné implementovat v rámci implementace řešení. Parametr *ExpirationDurationInMinutesKey* určuje expiraci cache v minutách. V případě první varianty cache k expiraci dochází při recyklaci procesu.
###### Cache bypass
V případě nutnosti provést dotaz přímým průchodem cache bez kešovaných dat je možné použít objekt *CacheBypass*:
```csharp
using(new CacheBypass())
{
	DoSomeWorkWithCacheByPass();	
}
```
Metoda volaná v tomto bloku bude průchozí pro všechny instanciované cache. 
Průchodnost je validní na úrovni vlákna zpracování (ThreadStatic).

##### DayOff provider
Komponenta poskytuje podporu pro státní svátky. Na webu, na kterém se očekává použití této funkce je nutné aktivovat funkci pro státní svátky ze společného WSP balíčku.
Aktivací dojde k vytvoření předdefinovaného seznamu s Českými státními svátky.
```csharp
public interface IDayOffProvider : IAutoRegisteredService
    {
        bool IsDayOff(CentralClock clock, int lcid = 1029, string dayOffProviderUrl = null);
        int GetDayOffsCountFromDateRange(CentralClock from, CentralClock to, IList<DayOfWeek> exceptList, int lcid, string dayOffProviderUrl = null);
    }
```
##### Security
###### Skupiny
Objekty reprezentující příslušné skupiny vznikají implementací bázové třídy *ApplicationGroup*:
```csharp
   public abstract class ApplicationGroup
    {
        public abstract string Name { get; }
        public abstract string Description { get;}
        public virtual string ApplicationName => Constants.SOLUTION_NAME;
        /// <summary>
        /// Name provider with parameter as web title
        /// </summary>
        public virtual Func<string, string> CustomNameProvider { get; }
    }
```
Každá skupina obsahuje název, popis a řešení pro které je implementována. Překlad názvu skupiny z doménového objektu zajišťuje služba *IHierarchicalGroupProvider*:
```csharp
public interface IHierarchicalGroupProvider : IAutoRegisteredService
    {
        SectionGroupDefinition GetDefinition(string webTitle, ApplicationGroup groupKind);
    }
```
Defaultně je název každé skupiny překládán jako:

**{0} {1} {2}**, kde
- {0} = Název aplikace
- {1} = Název skupiny
- {2} = Název webu

Defaultní name provider je možné přepsat implementací vlastnosti *CustomNameProvider* bázového objektu *ApplicationGroup*. Jako parametr provideru vstupuje název webu. Příklad skupiny s vlastním name providerem:
```csharp
public class ApplicationWriterGroup : ApplicationSolutionGroup
    {
        public static string GroupNameCustom = "Application writer custom";
        public override string Name => "Writer";
        public override string Description => "Writer can write";
        public override Func<string, string> CustomNameProvider => webTitle => GroupNameCustom;
    }
```
Instalaci příslušných skupin je možné implementovat definicí bázového objektu *BaseUserGroupsSetup* a implementací vlastnosti *membershipSetup*, která definuje jednotlivá oprávnění:
```csharp
public class SiteGroupMembershipSetup : BaseUserGroupsSetup
    {
        public SiteGroupMembershipSetup(IHierarchicalGroupProvider hierarchicalGroup) : base(hierarchicalGroup)
        {
        }

        protected override IDictionary<ApplicationGroup, Func<SPWeb, SPRoleDefinition>> membershipSetup =>
            new Dictionary<ApplicationGroup, Func<SPWeb, SPRoleDefinition>>
            {
                { Setup.Membership.ApplicationGroups.Reader, w => w.RoleDefinitions.GetByType(SPRoleType.Reader) },
                { Setup.Membership.ApplicationGroups.Writer, w => w.RoleDefinitions.GetByType(SPRoleType.Contributor) }
            };
    }
```
###### Security context
Security context je objekt reprezentující aktuální datový context SharePoint obalující několik základních vlastnosí na každém http requestu.
Jsou implementovány dva typy security contextů *ISecurityContext*:

|Typ   | Příklad použití   |
| ------------ | ------------ |
|FrontEndSecurityContext   | Kdekoliv , kde se pracuje s frontendem, aplikační stránky, webové části, webové metody, obecně ve všech aplikačních poolech webových aplikací.   |
|TimerJobSecurityContext | Časové úlohy, konzolové aplikace, testy |

FrontEndSecurityContext je automaticky v rámci IoC registrován do frontend kontejneru analogicky TimerJobSecurityContext.

Vlastnosti:

|  Název  | Význam   |
| ------------ | ------------ |
|IsFrontend   | Zda se jedná o frontend context   |
|CurrentUserName   | Login aktuálně přihlášeného uživatele  |
|CurrentWebTitle   | Název aktuálního webu  |
|CurrentWebUrl   | URL aktuálního webu   |
|CurrentUserAdmin   | Zda je aktuálně přihlášený uživatel site admin  |
|CurrentUser   | UserInfo objekt reprezentující aktuálního uživatele   |
|CurrentLocation   | SectionDesignation objekt reprezentující aktuální web  |
|IsSystem | Zda aktuálně přihlášený uživatel je systémový účet|

Metody:

|Název | Význam   |
| ------------ | ------------ |
| GetCurrentUserRoles | Vrátí seznam skupin, kterých je aktuálně přihlášený uživatel členem   |
| GetUserGroups  |  Totožné jako *GetCurrentUserRoles* |
| IsCurrentUserInGroup  | Zjistí zda je aktuálně přihlášený uživatel členem dané skupiny  |
| IsCurrentUserInGroupRecursive  | Totožné jako IsCurrentUserInGroup navíc vnořené skupiny rekurzivně  |
| IsUserInGroup  |  Zjistí zda je uživatel členem dané skupiny |
| CurrentUserInGroups  | Zjistí zda aktuálně přihlášený uživatel je členem skupin   |
| UserInGroups  | Totožné jako *CurrentUserInGroups* pro uživatele zadaného jako parametr |
| IsEmptyGroup  | Zjistí zda je skupina prázdná (neobsahuje žádné členy)   |
| GetItemRoleDefinition  | Vrací seznam aplikačních rolí (výčet) pro aktuálně přihlášeného uživatele   |

Nad objektem typu *SPUser* je vytvořena abstrakce v podobě jednotného rozhraní *IPrincipalInfo*. Existují dvě implementace:
1. *GroupInfo* - Objekt reprezentující skupinu SharePoint
2. *UserInfo* - Objekt reprezentující uživatele SharePoint

##### Sql Data access
Knihovna obsahuje jednoduchý database-first repository pattern přístupu k SQL datům a využívá ORM **Entity framework 6.X**. 
Database-first přístup je zvolen na základě zvyklostí a již odzkoušeném a bezproblémovém způsobu implementace v zákaznických prostředích.

Při database-first přístupu je nutné celý databázový model vygenerovat pomocí ADONET entity data modelu.
Model lze vložit do projektu přes:

> Add -> New item -> Data -> ADONET Entity Data Model

Pomocí průvodce postupně vytvořit context pro existující databázi.
Kód přístupu k SQL datům by měl být implementován ve vrstvě datové implementace se sufixem *DataAccess.Sql*.
Pro práci s datovou vrstvou společné knihovny je nutné pomocí NUGET přidat referenci na interní balíček společné knihovny:

> RhDev.SharePoint.Common.DataAccess.Sql  

###### DBContext
Při database-first přístupu je kontextová třída (třída dědící DbContext) databáze generována automaticky designerem ADONET.
Při tomto přístupu je nutné vždy zajistit aby tato předgenerovaná třída obsahovala override konstruktoru se string parametrem connection stringu, který je využíván při sestavování kontextových tříd vnitřními službami datového modelu.
Po každém update databáze nebo po prvním vygenerování je nutné override konstruktoru implementovat:

```csharp
  public partial class FooDatabase : DbContext
    {
        public FooDatabase(string connString)
            : base(connString)
        {
        }
        public FooDatabase()
            : base("name=FooDatabase")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
        public virtual DbSet<Foo> Foos { get; set; }
    }
```

###### Eager loading 
Přímý přístup pomocí repository patternu není kompatibilní s lazy loading a veškerá related data odkazující v relacích mezi entitami je třeba definovat při prvním přístupu pomocí přetížení čtecích metod.

```csharp
TStoreEntity ReadById(
	int id,
	IList<Func<TStoreEntity, object>> include = null);
```
```csharp
IList<TStoreEntity> Read(
	Expression<Func<TStoreEntity, bool>> lambda, 
	IList<Expression<Func<TStoreEntity, object>>> include = null, 
	bool? checkSingle = null);
```
```csharp
IList<TStoreEntity> ReadAll(
	IList<Expression<Func<TStoreEntity, object>>> include = null
);
```

Pomocí parametru *Include* je možné definovat loading 1-N entit. Příklad poutití:
```csharp
var book = 
		bookRepository.ReadAll(
			include: new List<Expression<Func<Book, object>>> { b => b.Author1 }
);
```
###### Lazy loading
Kod akceptující **lazy loading** je nutné volat pomocí služby:

```csharp
public interface IDatabaseAccessRepositoryFactory<TDatabase> 
	: IAutoRegisteredService where TDatabase : DbContext
    {
        T RunReturnAction<T>(Func<TDatabase, T> action);
        void RunAction(Action<TDatabase> action);
    }
```
Použití:
```csharp
IDatabaseAccessRepositoryFactory<TestEntitiesDatabase> dbFactory =
                container.Frontend.GetInstance<IDatabaseAccessRepositoryFactory<TestEntitiesDatabase>>();

            dbFactory.RunAction(db =>
            {
                var author = db.Authors.FirstOrDefault();
                var books = author.Books;
            });
```
###### Repository
Přístup k objektům standardním repository přístupem se vytváří rozšířením bázové abstraktní třídy *StoreRepositoryBase* a implementací rozhraní *IStoreRepository*

```csharp
public abstract class StoreRepositoryBase<TStoreEntity, TDatabase> 
		: DatabaseRepositoryBase<TStoreEntity, TDatabase>,
        IStoreRepository<TStoreEntity> where TStoreEntity : class where TDatabase : DbContext
    {
```
kde *TStoreEntity* je entitní objekt databáze vygenerovaný database-first ADONET generátorem a *TDatabase* je DBContext dané databáze generovaný taktéž. Datová třída reprezentující příslušnou entitu v databázi může vypadat následovně:

```csharp
public class BookRepository : 
StoreRepositoryBase<Book, TestEntitiesDatabase>,
IStoreRepository<Book>
    {
        public BookRepository(
		IDatabaseAccessRepositoryFactory<TestEntitiesDatabase> databaseAccessRepositoryFactory) :
		base(databaseAccessRepositoryFactory)
        {
        }
    }
```
Bázový konstrutkor repository vyžaduje službu *IDatabaseAccessRepositoryFactory* s generickým parametrem typu DbContextu dané databáze.

###### Autoregistrace
Přístup k datové entitě je možný pomocí autoregistrační factory resp metody *GetStoreRepository*:
```csharp
public interface IDataStoreAcessRepositoryFactory : IAutoRegisteredService
    {
        IStoreRepository<TEntity> GetStoreRepository<TEntity>() where TEntity : class;
    }
```
Podmínkou je aby daná entitní třída implementovala rozhraní *IAutoRegisterStoreRepository*: Příklad takové třídy:
```csharp
public class AuthorRepository : StoreRepositoryBase<SQL.Author, TestEntitiesDatabase>,
        IStoreRepository<Author>,
        IAutoRegisterStoreRepository
    {
        public AuthorRepository(
            IDatabaseAccessRepositoryFactory<TestEntitiesDatabase> databaseAccessRepositoryFactory) 
            : base(databaseAccessRepositoryFactory)
        {
        }
    }
```
Použití autoregistrační entity pomocí služby *IDataStoreAcessRepositoryFactory*:
```csharp
  var factory = container.Frontend.GetInstance<IDataStoreAcessRepositoryFactory>();
  var authorRepository = factory.GetStoreRepository<Author>();
```
V případě že daná entita není registrovaná jako auto-registační vývojář se o její vytvoření musí postarat sám pomocí explicitní factory.
###### Connection string
Connection string aplikační databáze by měl být přístupný jako šifrovaný konfigurační parametr v globální konfiguraci řešení. Společná knihovna obsahuje globální konfigurační objekt *GlobalConfiguration* obsahující vlastnost *ConnectionString*. Hodnota této vlastnosti je šifrována vnitřním encryptorem. Parametr connection stringu je obalen v třídě *ConnectionInfoFetcher*, který je registrován jako singleton a je validní a neměnný po celou dobu běhu procesu. Konstruktor přijímá daný objekt typu *GlobalConfiguration*, který využívá jako zdroj pro connection string. Tento konfigurační objekt může být v klientských řešeních rozšířen, jeho rozšíření je nutné zaregistrovat v kontejneru:
```csharp
For<SharePoint.Common.DataAccess.SharePoint.Configuration.Objects.GlobalConfiguration>().
	Use<RhDev.Customer.Solution.Common.Configuration.GlobalConfiguration>();
```
!!! Connection string musí být uložen v plném formátu včetně metadatových parametrů ADONET modelu:

```csharp
metadata=res://*/SQL.EntitiesModel.csdl|res://*/SQL.EntitiesModel.ssdl|res://*/SQL.EntitiesModel.msl;provider=System.Data.SqlClient;provider connection string="data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=|DataDirectory|\SQL\TestEntities.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework"
```

###### Oprávnění
Objektový model datové vrstvy Sql přistupuje do databáze impersonifikované účtem aplikačního poolu aplikace ve kterém běží výkoný kód.
Z tohoto důvodu je nutné během imlementace fyzické databáze do Sql toto oprávnění nastavit. 
Součástí servisních instalačních skriptů řešení vytvořeného ze společné šablony toto nastavení je implementováno ve skriptu *008_CreateDatabase.ps1*.
Při spuštění skriptu dojde k nastavení oprávnění *db_datareader* a *db_datawriter* pro účet aplikačního poolu instalované aplikace.

##### SharePoint Data access
Přístup k objektovému modelu SharePoint zajišťuje implementace jednoduchého repository patternu.
Každá třída reprezentující datovou entitu / SharePoint seznam dědí bázovou abstraktní generickou třídu *EntityRepositoryBase*:
```csharp
public class ApplicationConfigurationRepository :
EntityRepositoryBase<ApplicationConfiguration>, IApplicationConfigurationRepository
```
Generickým parametrem je typ entity daného seznamu, objekt jako potomek bázové entity *EntityBase*.

Třída reprezentující repository musí implementovat 3 základní metody předepisující transformaci surových datových objektů (řádku seznamů) na doménové objekty aplikace a to oběma směry:
```csharp
protected override void LoadData(SPListItem listItem, EntityDomainType entity)
protected override void CreateData(SPListItem listItem, EntityDomainType entity)
protected override void UpdateData(SPListItem listItem, EntityDomainType entity)
```
První metoda provádí transformaci surového objektu / řádku seznamu na entitní / doménový objekt a je volána ve chvíli kdy je dotazem načten dataset řádků. Druhá výše zmíněná 
metoda provádí přesný opak a je volána ve chvíli kdy je nutné data v seznamu vytvořit respektove modifikovat a je nutné transformovat doménové objekty do 
systémových objektů reprezentující řádky v seznamu. Poslední metoda je volána při modifikaci dat v seznamu a má typicky totožnou implementaci jako metoda *CreateData*.

###### Přístup se zvýšenými právy**
API bázové třídy **EntityRepositoryBase** umožňuje přístup se systémovým účtem pomocí override vlastnosti *RequiresElevation* :
```csharp
protected override bool RequiresElevation => true;
```
Jakákoliv čtecí / zapisovací operace proti seznamu bude impersonifikována systémovým účtem s nejvyššími právy.

###### Složková struktura seznamu
V rámci repository je možné definovat složkovou strukturu, do které se budou jednotlivé položky seznamu vytvářet.
Struktura se definuje overridem vlastnosti *FolderStructureProvider*:
```csharp
protected virtual Func<TEntity, string> FolderStructureProvider
```
Parametrem je objekt dané entity a výstupem musí být složková struktura definovaná jako znakem dopředné lomítko ('/') oddělená stringová reprezentace dané složky:
**A/B/C**

V dané repository je nutný override vlastnosti *IsFolderStructurable*:
```csharp
protected override bool IsFolderStructurable => true;
```
Jako defaultní provider pro složkové dělení při aktivní vlastnosti *IsFolderStructurable* je implementován provider dle datumového / časového dělení jednotlivých entit:
- Rok
  - Měsíc
    - Den

Využívá napříkad LOG repository ve společné knihovně. Pro specifické případy je nutné definovat vlastní provider.

###### Factory
Pro přístup k jednotlivým repository pomocí objektového modelu se využívá služba *ICommonRepositoryFactory*:
```csharp
public interface ICommonRepositoryFactory : IAutoRegisteredService
    {
        IApplicationConfigurationRepository GetApplicationConfigurationRepository(string webUrl = null);
        IApplicationLogRepository<LogItem> GetApplicationLogRepository(string webUrl = null);
        IDayOffRepository GetDayOffRepository(string webUrl = null);
        T GetRepository<T, TEntity>(string webUrl = null) where TEntity : EntityBase;
    }
```
První 3 metody slouží pro přístup k 3 základním seznamům ze společné knihovny. Generický přístup k libovolné repository je realizován poslední variantou
kde jako první generický parametr vstupuje objekt reprezentující danou repository a druhý reprezentující typ entity dané repository.
Příklad:
```csharp
public class ApplicationConfigurationRepository :
EntityRepositoryBase<ApplicationConfiguration>, IApplicationConfigurationRepository
```

kde T = *ApplicationConfigurationRepository* a TEntity = *ApplicationConfiguration*
Konstruktor bázové třídy dané repository vyžaduje dva parametry:
- *webUrl* - URL webu, na které bude seznam vyhledáván
- *listFetcher* - Func SPWeb -> SPList - provider pro resoloving seznamu na daném webu

Společná knihovna obsahuje 3 předdefinované providery definované ve statické tříde *ListFetcher*:
1. ForGuid - resolving dle ID seznamu (ID musí být předem na seznamu implementačně vytvořeno)
2. ForTitle - resolving dle názvu seznamu
3. ForRelativeUrl - resolving dle relative url daného seznamu

Pro výkonově kritické aplikace je vhodné použít první variantu.

Url jednotlivých seznamů injektovaných v rámci factory je načítána z farmové konfigurace, kde je typicky uložena URL aplikace pokud není explicitně pomocí parametru *webUrl* definováno jinak.
Konstruktor příslušné repository může vypadat následovně:
```csharp
public ApplicationConfigurationRepository(string webUrl)
            : base(webUrl, ListFetcher.ForRelativeUrl(Config.Lists.APPCONFIGURL))
        {
        }
```

##### Test Fx
Vnitřní testovací framework umožňuje provádět jednotkové / integrační testování. Jádrem testovacího frameworku je AutoMocker objekt poskytující rozhraní pro jednotkové testování komponent a mokování všech závislostí v kontejnerovém stromu. Mokovací strom závislostí zajišťuje komponenta *NSubstitute*, která v podstatě vystupuje jako service locator pro jednotlivé mokované závislosti.
Celá logika je obalena abstraktní generickou třídou *UnitTestOf* jejímž generickým parametrem je právě testovaný objekt. Příklad jednotkové testu DayOff provideru:
```csharp
[TestClass]
    public class DayOffTest : UnitTestOf<DayOffProvider>
    {
        protected override void Setup()
        {
            base.Setup();
            var crp = Mocker.Get<ICommonRepositoryFactory>();
            crp.GetDayOffRepository().ReturnsForAnyArgs(new DayOffMockList());
        }

        [TestMethod]
        public void IsTodayDayOff()
        {
            var sut = SUT;
            var result = sut.IsDayOff(CentralClock.FillFromDateTime(new System.DateTime(2020, 8, 10)));
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsTodayDayOffWithNoRepeat()
        {
            var sut = SUT;
            var result = sut.IsDayOff(CentralClock.FillFromDateTime(new System.DateTime(2020, 8, 11)));
            result.Should().BeFalse();
        }
    }
```
Pro snažší asserci jednotlivých výsledků je použito fluent api externí knihovny *FluentAssertions*.

##### Instalace / upgrade feature
Kódová instalace featur probíha standarními prostředky pomocí event receiverů. 
Společné rozhraní knihovny nabízí podporu pro event receivery v podobě abstraktní třídy *FeatureReceiverBase*. Každý třída reprezentující event receiver dědí tuto třídu:
```csharp
public abstract class FeatureReceiverBase<TScope, TInstallation> 
: SPFeatureReceiver where TInstallation : FeatureInstallation<TScope> where TScope : class
```
Bázová třída má 2 generické parametry:
1. *TScope* - scope dané featury
	- SPWeb - scope v rámci webu
	- SPSite - scope v rámci kolekce webů
	- SPWebApplication - scope v rámci webové aplikace
	- SPWebService - scope v rámci farmy
2. *TInstallation* - objekt implementující logiku dané featury

Příklad třídy implementující logiku receiveru pro TraceLogger daného řešení:
```csharp
public class SolutionTracingInstallation : FeatureInstallation<SPWebService>
    {
        private readonly ITraceLogger traceLogger;
        public SolutionTracingInstallation(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;
        }
        protected override void DoExecuteInstallation(SPWebService scope)
        {
            traceLogger.Register();
        }
        protected override void DoExecuteUninstallation(SPWebService scope)
        {
            traceLogger.Unregister();
        }
    }
```
Výsledná třída pro definici event receiveru dané featury potom bude vypadat následovně:
```csharp
public class FarmEventReceiver : FeatureReceiverBase<SPWebService, SolutionTracingInstallation>
    {
        public FarmEventReceiver() : base(o => IoC.Get.Frontend.BuildUp(o))
        {
        }
    }
```
Protože objekt bázového event receiveru není sestaven standardně instanciací dané služby ale její dědičností a sestavení je nutné provést při volání aktivačních / deaktivačních metod (konstruktor receiveru je volán při instalaci řešení, kdy není 100 procent zaručeno, že GAC obsahuje pslední verze DLL) nikoliv v konstruktoru je nutné bázovému konstruktoru předat předpis pro IoC sestavení.

------------

Pro upgrade featur by klientská řešení měla využívat standardní prostředky deklarativní definice. Pro kódový upgrade featur je nutné definovat pomocí standardního elementu *VersionRange* v šabloně dané featury:
```xml
    <VersionRange BeginVersion="0.0.0.0" EndVersion="2.1.0.0">
      <CustomUpgradeAction>
        <Parameters>
          <Parameter Name="FeaturePart">Common.ListView</Parameter>
          <Parameter Name="TargetVersion">2.1.0.0</Parameter>
        </Parameters>
      </CustomUpgradeAction>
    </VersionRange>   

```
Každá kódová upgrade akce musí obsahovat dva povinné parametry:
1. *FeaturePart* - určuje čeho se daný upgrade týká, libovolný text
2. *TargetVersion* - určuje pro jakou verzi je daný upgrade určen, stringový zápis verze ve formátu **A.B.C.D**

Samotná kódová akce je poté definována implementací generického rozhranní *IFeatureUpgradeAction*:
```csharp
    public class CustomUpgradeAction : IFeatureUpgradeAction<SPWeb>
    {
        private readonly IFoo Foo;
        public string FeaturePart
        {
            get { return "Common.ListView"; }
        }
        public Version TargetVersion
        {
            get { return new Version(2, 1, 0, 0); }
        }
        public CompanySectionListFeatureUpgrade(IFoo foo)
        {
            	this.Foo = foo;
        }
        public void ExecuteUpgrade(SPWeb web)
        {
			Foo.DoAction();
        }
    }
```
Generický parametr musí odpovídat scope dané featury, třída musí být public a implementovat oba parametry z XML definice. Výkoný kód je poté implementován v metodě *ExecuteUpgrade*.

Upgrade jednotlivých featur je volán explicitně pomocí metody *SPFeature.Upgrade(bool)*. Metoda by měla být volána prostřednictvím servisních skriptů běhěm instalace / upgrade daného řešení.

##### Multithreading
Knihovna obsahuje podporu pro vícevláknový přístup nad konkrétní repository nebo libovolnou custom službou. 
Přístup je možný prostřednictvím služby *IConcurrentDataAccessRepository*:

```csharp
public interface IConcurrentDataAccessRepository : IAutoRegisteredService
    {
        void UseRepository<T, TEntity>(Action<T> a, string webUrl = null)
            where T : IEntityRepository<TEntity>, ISynchronizationContextService
            where TEntity : EntityBase;

        void UseService<T>(Action a) where T : ISynchronizationContextService;
    }

```

Následující volání zajistí zámek přístupu (readi i write) nad repository *SectionSettingsRepository*:
```csharp
IConcurrentDataAccessRepository concurrentDataAccessRepository = IoC.Get.Backend.GetInstance<IConcurrentDataAccessRepository>();
concurrentDataAccessRepository.UseRepository<SectionSettingsRepository, SectionSettings>(r => 
            {
                DoSomethingWithLockOnTheSectionSettingsRepository();
            });
```
Podmínkou je že daná repository implementuje rozhraní *ISynchronizationContextService*. Stejně tak tomu je i v případě aplikace zámku nad custom službou:

```csharp
concurrentDataAccessRepository.UseService<AdAccessService>(() =>
                {
                    DoSomethingWithLockOnCustomAdAccessService();
                });
```

#### Externí knihovny
Řešení společné knihovny obsahuje několik externích knihoven. Tyto knhovny jsou distribuovány z oficiálnoho NUGET feedu a nasazovány na farmu SharePoint pomocí speciálního externího WSP. Z důvodu kompatibility mezi jednotlivými prostředími a nejednotnosti ve verzování některých NUGET balíčků jsou verze externích knihoven zafixovány na odzkoušené a stabilní verze:

| Název balíčku  | Význam balíčku  | Assembly Verze knihovny   | Nuget package verze knihovny |Typ |
| ------------ | ------------ | ------------ | ------------ | ------------ |
| FluentRibbon   | Podpora pro kódovou definici Ribbonu v klientských řešeních   | 4.0.0.0   |4.0.0|Nasazení na farmu SharePoint|
| StructureMap-signed    | Podpora pro IoC a testování   |3.1.6.191   |3.1.6.191|Nasazení na farmu SharePoint|
| StructureMap.web-signed    | Rozšíření bázové knihovny o některé extension metody   |3.1.6.191   | 3.1.6.191|Nasazení na farmu SharePoint|
| EntityFramework   | ORM   | 6.0.0.0   |6.2.0|Nasazení na farmu SharePoint|
| NSubstitute   | Podpora pro unit testing   |-   |4.2.2| Developer balíček, verze nerozhoduje|
| FluentAssertion   | Propora pro unit testing   |-   |6.0.0-alpha0001|Developer balíček, verze nerozhoduje|
| Rhino.Mocks   | Propora pro unit testing   |-   |3.6.1|Developer balíček, verze nerozhoduje|
| StructureMap.AutoMocking   | Propora pro unit testing   |-   |3.1.6.186|Developer balíček, verze nerozhoduje|

#### WSP řešení a instalace společné knihovny na farmě SharePoint
Řešení je distribuováno jako skupina WSP balíčků pro farmu SharePoint kompatibilní s verzemi **2016** a **2019**. Balíčky včetně instalační složky se v prostředí RhDev - BP nacházejí v UNC:
> \\dx-vyvoj\release\Zákazníci\RhDev\_internal\common\install\[**verze**]

Jednotlivé verze jsou postupně vydávány na základě výsledků kontinuální integrace jednotlivých buildů (tento proces zatím není automatizovaný). Seznam verzí obsahuje i složku *_latest* s poslední (aktuální) verzí knihovny.
Obsah adresářové struktury:
```
install
└─latest
└─1.20.246.113    
            |
		   └──Deploy
		   │          └─Helpers──
		   │		  │         │─FeatureHelper.ps1
		   │		  │         │─SecureStoreServiceHelper.ps1
		   │		  │         │─SolutionDeploymentHelper.ps1
		   │		  │         │─SqlHelper.ps1
		   │		  │──Others──
		   │		  │         │─ExecuteInstallation.ps1
		   │		  │         │─UpdateApplication.ps1
		   │		  │──Packages
		   │		  │         │─RhDev.SharePoint.wsp
		   │		  │         │─RhDev.SharePoint.Externals.wsp
		   │		  │──000_Init.ps1
		   │		  │──001_AddAndDeployCORE.ps1
		   │		  │──002_AddAndDeployJOBS.ps1
		   │		  │──003_AddAndDeployExternals.ps1
		   │		  │──004_EnableFeatures_Global.ps1
		   │		  │──801_RestartTimer.NOAUTORUN.ps1
		   │		  │
		   └──Upgrade
		   │		  │─ExecuteUpgradeScripts.ps1
		   │
```
##### Instalace
Instalace knihovny se spouší z instalační složky *Deploy\Others\ExecuteInstallation.ps1* příslušné verze.
Pomocí jednoduchého UI průvodce je v rámci instalace nutné postupně spustit jednotlivé kroky dle pokynů:

|Skript / krok   | Význam   |
| ------------ | ------------ |
|001_AddAndDeployCORE.ps1  | Přidání a deploy core řešení knihovny   |
|002_AddAndDeployJOBS.ps1   | Skript se nespouští, je možné přeskočit ('n') nebo ('ESC'), skript je připraven na verzi obsahující časové úlohy. Knihovna v současné verzi 1.20.246.113 zatím žádné časové úlohy neobsahuje |
|003_AddAndDeployExternals.ps1   |Přidání a deploy balíčku s externími knihovnami   |
|004_EnableFeatures_Global.ps1   | Aktivace featury se společným TraceLoggerem   |
|801_RestartTimer.NOAUTORUN.ps1   | Recyklace OWS, není nutné spouštět   |

##### Update
Update řešení se provádí spuštěním skriptu *Deploy\Others\UpdateApplication.ps1*, kterým dojde k update core balíčku řešení stadardními prostředky SharePoint.

##### Verze .NET
Z důvodu sjednocení knihovny pro verzi 16 i 19 je knihovna postavena na jednotné verzi .net framework **4.7.2**.
Verze 4.7.2 vyžaduje běhové prostředí CLR **4.0**. Před instalací je nutné zajistit že daný framework je na cílovém prostředí nainstalovaný.
Jak zjistit jaký Framework je na cílovém prostředí nainstalovaný:
> https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed?redirectedfrom=MSDN#detect-net-framework-45-and-later-versions

##### Featury
Řešení obsahuje 3 featury:

|Název v SP HIVE  |CZ lokalizace   | ID   | Význam   |
| ------------ | ------------ | ------------ | ------------ |
|RhDev.SharePoint_Web   | RhDev společné  |d691910d-4c56-49c5-a87c-0c7c3cd1e83e   | Seznamy konfigurace, log a státní svátky. Featura je závislá na *RhDev.SharePoint_Site*, závislost je řešena automaticky kódovou aktivací  |
|RhDev.SharePoint_Site   |RhDev společné   |e0f3966e-25d5-4427-959d-6f5cf5c19095   | Typy obsahu a sloupce pro seznamy   |
|RhDev.SharePoint_Farm   |RhDev Common (pevně bez lokalizace)   |7a2d9721-7980-45ca-b66e-c14b34c8a037   | Obsahuje společný trace logger řešení   |

##### Pravidla pro vývoj
Při vývoji nebo rozvoji knihovny je třeba dbát na některá pravidla, která je třeba z důvodu kompatibility se zákaznickými řešeními dodržovat.
###### Breaking change
Breaking change není při vývoji knihovny žádoucí a nemělo by k němu při úpravách respektive vývoji docházet.
Společná knihovna může být sdílena více zákaznickými řešeními a různý kód v rozdílných verzích knihovny může způsobit nežádoucí chování v zákaznickcých knihovnách.
Příklad úpravy metody:
> Foo.cs (1.0.0)

```csharp
public void DoSomething(int a, int b)
{
	DoNothing();
}
```

> Foo.cs (2.0.0-bad)

```csharp
public void DoSomething(int a, int b, string op)
{
	DoNothing();
}
```
Typickým příkladem breaking change je rozšíření kontraktu metody. Manipulace s parametry metody by měla být řešena například pomocád defaultních parametrů nebo vytvořením nové metody a nastavením obsolote na původní.

> Foo.cs (2.0.0-better1)

```csharp
public void DoSomething(int a, int b, string op = "modulo")
{
	DoNothing();
}
```

> Foo.cs (2.0.0-better2)

```csharp
[Obsolete("This method is obsolote, please use DoMath instead")]
public void DoSomething(int a, int b)
{
	DoNothing();
}

public void DoMath(int a, int b, string op)
{
	DoNothing();
}
```

###### Verzování
U verzování knihoven společného projektu musí platit:
1. Assembly verze (AssemblyVersion) core knihoven musí být nastaveno vždy na verzi **1.0.0.0**
	- Platí pro knihovny ze společného projektu i pro knihovny zákaznických řešení. Společné verzování je důležité z důvodu jednotného systému při sestavování IoC kontejneru.
2. File verze dané knihovny (AssemblyFileVersion) nerozhoduje a může být nastavena libovolně. To platí i pro AssemblyInformationalVersion (používá NUGET) v případě že by například klientská knihovna byla vystavena jako NUGET balíček.  Ve společné knihovně je řešeno automaticky pomocí CI. Obecně může platit že File a AssemblyInformational verze jsou totožné.

## Šablona projektů
Společná šablona umožňuje strukturovat zákaznické projekty podle předem vygenerované skupiny souborů a konfiguračních parametrů jednotně pro všechna zákaznická řešení.
Šablona je dostupná pro verze **SharePoint 2016** a **SharePoint 2019**.
### Instalace šablony
Soubor šablony dostupný v UNC:

> \\dx-vyvoj\release\Zákazníci\RhDev\_internal\common\template\RhDev.Zakaznik.Reseni.zip

je nutné nakopírovat do složky s šablonami v příslušné instalaci verze VS, která je dostupná:
> [User system folder]\Documents\Visual Studio [Verze]\Templates\ProjectTemplates\

Ve VS je poté nová šablona dostupná při vytváření nového projektu:
![](https://storageaccountscdlpa8ad.blob.core.windows.net/imgcontainer/RhDevTemplate.PNG)

#### Název projektu
Při vytváření projektu podle šablony by měl název projektu dodržovat jmennou konvenci:

**RhDev.[Zákazník].[Řešení]**

kde *Zákazník* je zkratka zákazníka bez diakritiky (MP, Globus, AVE) a *Řešení* je jednoznačný název řešení (DMS, Vouchers, NotificationCenter). Po aplikaci šablony s názvem projektu **A.B.C** dojde k vytvoření následující struktury řešení:

![](https://storageaccountscdlpa8ad.blob.core.windows.net/imgcontainer/RhDevTemplateProject.PNG)
Řešení obsahuje následující projekty:

| Projekt  | Význam a obsah projektu   |
| ------------ | ------------ |
| A.B.C.Common   | Obsahuje základní nastavení projektu, konstanty, definici aplikačních skupin, trace loggeru a IoC kontejneru |
| A.B.C.Common.DataAccess   | Šablona neobsahuje žádný výkoný kód, připraveno pro datovou abstrakci common komponenty |
| A.B.C.Common.DataAccess.SharePoint  | Obsahuje konfigurační objekty, časové úlohy, instalace feature receiverů, setup pro skupiny, trace logger  |
| A.B.C.Common.Impl  |  Šablona neobsahuje žádný výkoný kód, připraveno pro business logiku common komponenty  |
| A.B.C.Common.Test  |  Projekt s jednotkovými / integračními testy  |
| A.B.C  |  SharePoint projekt daného zákaznického řešení obsahující základní strukturu projektu, včetně servisních skriptů. |
| A.B.C.TimerJobs  |  SharePoint global-scoped projekt s instalací časových úloh řešení. |

#### Popis SharePoint řešení
Core balíček SharePoint projektu obsahuje bázovou strukturu s několika předgenerovanými soubory:
![](https://storageaccountscdlpa8ad.blob.core.windows.net/imgcontainer/TemplateSolution.PNG)

Řešení obsahuje web site a farm scoped featury. Farm featura respektiove její receiver obsahuje přípravu pro instalaci trace loggeru klientského řešení.
Řešení obsahuje dva soubory s resources. Defaultní český a jeden překlad *en-US*. Toto nastavení předpokládá defaultní jazyk kolekce nebo webu daného řešení v *cs-CZ*.
Řešení s časovými úlohami obsahuje jednu webapplication scoped featuru s instalací těchto úloh.

##### Servisní skripty
Každé řešení založené na společné šabloně obsahuje sadu servisních / implementačních skriptů ve struktuře a chování totožném v popisu 
[WSP řešení a instalace společné knihovny na farmě SharePoint](#wsp-řešení-a-instalace-společné-knihovny-na-farmě-sharepoint)

Navíc jsou zde skripty:

|Skript / krok   | Význam   |
| ------------ | ------------ |
|003_SetupSiteCollection.ps1  | Setup / vytvoření kolekce webů či webu pro dané řešení |
|004_ConfigureFarm.ps1   | Konfigurace farmových vlastností (URL aplikace daného řešení) |
|003_AddAndDeployExternals.ps1   |Přidání a deploy balíčku s externími knihovnami   |
|005_EnableFeatures.ps1   | Aktivace featur řešení na daném webu včetně aktivace závislých featur ze společné knihovny |
|006_ConfigureConfiguration.ps1   | Konfigurace řešení. Instalace konfiguračbních parametrů řešení |
|008_CreateDatabase.ps1   | Vytvoření datové struktury řešení, instalace a konfigurace databáze |
|009_AddAdminAsApplicationAdmin.ps1   | Umožňuje přidat site admina do aplikační administrátorské role, pokud je vytvořena  |

Konfigurace dané instalace se provádí definicí konfiguračních parametrů v init skriptu (Konfigurovatelné parametry jsou označeny tagem *#ENV*, ostatné parametry jsou předgenerované a pevně určené a k jejich změně by nemělo docházet):

> 000_Init.ps1

|Parametr   | Význam   |
| ------------ | ------------ |
|DatabaseName  | Konfigurace databáze - Název aplikační databáze |
|DatabaseServer  | Konfigurace databáze - databázový server / instance |
|DBConnectionStringWithEDMX  | EDMX schema databáze vygenerované ADONET generátorem |
|Jobs.Empty.Server  | Bind jobu řešení na server, uvádí se název serveru, v řešení je vygenerována kostra jednoho testovacího jobu, každý nový job by měl obsahovat svou konfiguraci. |
|App.Site  | Absolutní URL kolekce webů řešení, nový nebo existující |
|App.Web  | Absolutní URL webu řešení, nový nebo existující |
|App.Name  | Název webu řešení |
|Administrators.Primary   | Primární administrátor kolekce webů řešení, pokud je kolekce nová pokud ne ignoruje se |
|Administrators.Secondary   | Sekundární administrátor kolekce webů řešení, pokud je kolekce nová pokud ne ignoruje se |
|WebApplication.Url   | URL webové aplikace řešení, **aplikace musí již existovat** |
|Administrators.Secondary   | Sekundární administrátor kolekce webů řešení, pokud je kolekce nová pokud ne ignoruje se |

#### Konfigurace a instalace logování
V rámci šablony je předpřipraven vlastní trace logger, kterým je možné nahradit trace logger ze společné knihovny.
Jeho konfigurace se provádí ve vrstvě *Common* ve třídě */Setup/Tracing/TraceCategories.cs*:
```csharp
    public class TraceCategories
    {
        public static TraceCategory Database => 
		new TraceCategory(Const.SOLUTION_DISPLAY, "Database", TraceSeverity.Medium, EventSeverity.Verbose);
        public static TraceCategory Service => new TraceCategory(Const.SOLUTION_DISPLAY, "Service");
    }
```
V tomto souboru je nutné nadefinovat všechny trace kategorie daného řešení. Každá kategorie je instancí třídy *TraceCategory*.
```csharp
public TraceCategory(
	string area,
	string category,
	TraceSeverity traceSeverity,
	EventSeverity eventSeverity);
```

- **Area** - oblast pod kterou jsou seskupeny všechny kategorie daného řešení
- **category** - název kategorie
- **TraceSeverity** - severity pro ULS
- **EventSeverity** - severity pro EventLog

Veškeré definované kategorie je poté nutné specifikovat v konfiguračním souboru Trace loggeru v *TraceConfiguration.cs*:
```csharp
public static class TraceConfiguration
    {
        public static DiagnosticsServiceConfiguration GetDiagnosticsServiceConfiguration =>
            new DiagnosticsServiceConfiguration(Const.SOLUTION_DISPLAY, new List<TraceCategory>
            {
                TraceCategories.Database,
                TraceCategories.Service
            });
    }
```

Poté je nutné vytvořit třídu vlastního loggeru odvozením od bázové systémové třídy *SharePointTraceLogger* a implementací vlastnosti *GetConfiguration* vložit referenci na konfiguraci z předchozího kroku:
```csharp
public class SolutionTraceLogger : SharePointTraceLogger
    {
        public override DiagnosticsServiceConfiguration GetConfiguration => TraceConfiguration.GetDiagnosticsServiceConfiguration;
    }
```

Tento logger je nutné zaregistrovat v rámci IoC a přepsat tím tak původní systémový logger ze společné knihovny. Registraci je možné provést ve vrstvě *DataAccess.SharePoint* nebo *impl*
> DefaultCompositionConfiguration.cs

```csharp
For<ITraceLogger>().Use<SolutionTraceLogger>();
```

Poté je ještě nutné v rámci event receiveru farmové featury trace logger zaregistrovat, čímž dojde k jeho instalaci na farmu SharePoint:
> Common.DataAccess.SharePoint/Installation/Tracing/SolutionTracingInstallation.cs
```csharp
public class SolutionTracingInstallation : FeatureInstallation<SPWebService>
    {
        private readonly ITraceLogger traceLogger;
        public SolutionTracingInstallation(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;
        }
        protected override void DoExecuteInstallation(SPWebService scope)
        {
            traceLogger.Register();
        }
        protected override void DoExecuteUninstallation(SPWebService scope)
        {
            traceLogger.Unregister();
        }
    }
```

#### Konfigurace a instalace aplikačních skupin
Derfinice jednotlivých aplikačních skupin se nachází ve vrstvě *Common* v */Setup/Membership*:
Šablona obsahuje jednu předdefinovanou skupinu *Čtenář*:
> Reader.cs

```csharp
public class Reader : ApplicationSolutionGroup
    {
        public override string Name => "Čtenář";

        public override string Description => "Může číst";
    }
```

Každá aplikační skupina daného řešení dědí ze společné solution-specific třídy *ApplicationSolutionGroup*.
Seznam rolí příslušných skupin na daném webu se definuje pomocí setup souboru ve vrstvě *DataAccess.SharePoint* v souboru */Instrallation/Membership/SiteGroupMembershipSetup.cs*. Pomocí override vlastnosti *membershipSetup* je nutné definovat role pro jednotlivé skupiny. Instalace je automaticky definována v instalační třídě */Installation/SiteInstallation.cs*:

#### Konfigurace IoC
IoC kontejner v podobě statické fasády pro oba typy je vytvořen ve vrstvě *Common* v souboru *Setup/IoC.cs*:
Použití kontejneru pomocí předgenerované fasády je následující:

```csharp
FarmConfiguration = IoC.Get.Backend.GetInstance<FarmConfiguration>();

TraceLogger = IoC.Get.Frontend.GetInstance<ITraceLogger>();

IoC.Get.Frontend.BuildUp(o)
```
Předgenerovaná definice kontejneru se nachází v souboru */Setup/CompositionDefinition.cs*. Při implementaci nových komponent respektive vrstev je nutné tyto zaregistrovat zde.
Jak definovat požadavky pro IoC pomocí vnitřního builderu je popsáno v kapitole [IoC](#ioc)

### NuGet
Reference na externí komponenty včetně společné knihovny je realizována prostřednictvím NuGet balíčkovacího systému.
Při rebuild řešení nebo po explicitním operaci restore dojde k instalaci všech balíčků definovaných v jednotlivých projektových souborech. Vedle oficiálního nuget feedu je nutné přidat referenci také na privátní feed, ze kterého jsou instalovány knihovny ze společného řešení.

#### Privátní Feed
Nový feed lze přidat ve VS přes:

> Tools -> Options ->NuGet Package Manager ->Package Sources

Vpravo nahře kliknout na tlačítko "PLUS" a přidat:
- **Name** =Libovolný název reprezentující daný feed (př. AzureArtifacts)
- **Source** = https://pkgs.dev.azure.com/RhDev-BP/RhDev/_packaging/dxnet/nuget/v3/index.json

#### PackageReference
Z důvodu lepší kompatibility a sjedocení se systémem ve společné knihovně je vhodné nastavit PackageReference jako systém správy balíčků.
Nastavení se provádí:

> Tools -> Options ->NuGet Package Manager -> General -> Default package management format

zde nastavit na *PackageReference*.

#### Verze
Externí komponenty jsou nastaveny fixně na verze definované výše v kapitole [Externí knihovny](#externí-knihovny)
Verze pro společnou knihovnu je nastavena u všech komponent na iniciální verze **1.0.0**. O povýšení verze u konkrétního řešení rozhodne vývojář dle verze společné knihovny nainstalované u konkrétního zákazníka. Povýšení je nutné provést ve všech projektech a pro všechny komponenty společné knihovny:

![](https://storageaccountscdlpa8ad.blob.core.windows.net/imgcontainer/CommonLibVersion.PNG)

#### Kompatibilita
Kompatibilita společné knihovny a jednotlivých zákaznických řešení by měla dodržovat pravidla dle tabulky:

|NuGet verze společné knihovny nasazená u zákazníka  | Odkaz na verzi knihovny ze zákaznického řešení   |Popis |
| ------------ | ------------ | ------------ |
| X.Y.Z   | v = X.Y.Z   | Nejlepší případ verze knihoven jsou totožné|
| X.Y.Z   | v < X.Y.Z   | Verze odkazované knihovny je menší než instalovaná verze u yákazníka. Při zachování pravidel definovaných v kapitole [Pravidla pro vývoj](#pravidla-pro-vývoj) zejména breaking change a verzování je tento stav plně kompatibilní.|
| X.Y.Z  | v > X.Y.Z   | Nedoporučeno a nemělo by nastávat, nainstalovaná verze knihovny u zákazníka nemusí obsahovat nové funkce odkazované ze zákaznického řešení.|

Z důvodu lepší organizace při vývoji a úpravách zákaznických řešení je vhodné aby informace o verzích u jednotlivých zákazníků byly organizovány na jednom společném místě, tak aby se předešlo konfliktům s verzováním knihoven.







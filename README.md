# 🎨 Art Market Analytics & ML Valuation Backend

Արվեստի շուկայի վերլուծության և մեքենայական ուսուցման (ML.NET) միջոցով նկարների արժեքի կանխատեսման REST API հարթակ: Նախագիծը մշակված է **.NET 8**-ի հիման վրա՝ կիրառելով **Onion Architecture** (Մաքուր ճարտարապետություն) սկզբունքները:

## 🏛 Ճարտարապետություն (Onion Architecture)

Համակարգը բաժանված է 4 անկախ շերտերի՝ ապահովելով կոդի մաքրությունն ու թույլ կապակցվածությունը (Loose Coupling).

* **`ArtMarket.Domain`** — համակարգի միջուկը: Պարունակում է դոմենային էության մոդելը (`ArtworkRecord`) և ռեպոզիտորիայի ինտերֆեյսը (`IArtworkRepository`)՝ առանց արտաքին կախվածությունների:
* **`ArtMarket.Application`** — բիզնես տրամաբանության շերտ: Ներառում է վերլուծական սերվիսը (`ArtAnalyticsService`), ինտերֆեյսները (`IArtAnalyticsService`, `IPricePredictionEngine`) և տվյալների փոխանցման օբյեկտները (DTOs):
* **`ArtMarket.Infrastructure`** — տվյալների բազայի և ML մոդuli իրականացում.
  * **Persistence:** PostgreSQL բազայի կոնտեքստ (`ArtMarketDbContext`)՝ Entity Framework Core-ի միջոցով:
  * **Machine Learning:** գնի կանխատեսման շարժիչ (`MlPricePredictionEngine`)՝ հիմնված **ML.NET FastTree** ռեգրեսիայի վրա:
* **`ArtMarket.Web`** — REST API շերտ: Ներառում է կոնտրոլլերը (`ArtworksController`), Dependency Injection-ի կարգավորումները և **Swagger UI** ինտերակտիվ միջավայրը:

## 🛠 Տեխնոլոգիական ստեկ

* **Պլատֆորմ.** .NET 8 / ASP.NET Core Web API (C#)
* **Տվյալների բազա.** PostgreSQL + Entity Framework Core
* **Մեքենայական ուսուցում.** Microsoft.ML (`FastTree` Gradient Boosted Decision Trees)
* **API փաստաթղթավորում և թեստավորում.** Swashbuckle (Swagger UI)

## 🤖 Մեքենայական ուսուցման կոնվեյեր (ML.NET Pipeline)

Նկարների շուկայական արժեքի գնահատման համար կիրառվում է **FastTree** գրադիենտային բուսթինգի ալգորիթմը.
1. **`OneHotEncoding (Indicator)`** — տեքստային կատեգորիաների (`ArtistTier`, `Medium`) վերածում բինար թվային վեկտորների:
2. **`Concatenate`** — բոլոր հատկանիշների միավորում մեկ ընդհանուր `RawFeatures` վեկտորի մեջ (`WidthCm`, `HeightCm`, `CreationYear`, `ArtistPastSalesCount`):
3. **`NormalizeMinMax`** — թվային արժեքների մասշտաբավորում `[0, 1]` միջակայքում՝ հատկանիշների կշիռները հավասարակշռելու համար:
4. **`Fallback Valuation`** — պահուստային մաթեմատիկական բանաձև Cold Start իրավիճակների համար (երբ բազայում տվյալները դեռ բավարար չեն):

## 📡 Հիմնական API Endpoints

| Մեթոդ | Հասցե | Նկարագրություն |
| :--- | :--- | :--- |
| `POST` | `/api/Artworks/import-csv` | CSV դատասեթի հոսքային ներմուծում PostgreSQL և ML մոդելի ավտոմատ վերավարժեցում |
| `GET` | `/api/Artworks/analytics` | Արվեստի շուկայի ագրեգացված վիճակագրություն և գնային թրենդներ |
| `GET` | `/api/Artworks/recent` | Վերջին ավելացված նկարների ցանկի ստացում բազայից (`?count=50`) |
| `POST` | `/api/Artworks/predict` | Նկարի շուկայական արժեքի կանխատեսում (USD)՝ ըստ պարամետրերի |

## 🚀 Ինչպես գործարկել նախագիծը

1. Կարգավորել PostgreSQL միացման տողը (`DefaultConnection`) `ArtMarket.Web/appsettings.json` ֆայլում:
2. Կիրառել EF Core միգրացիաները.
   ```bash
   dotnet ef database update --project ArtMarket.Infrastructure --startup-project ArtMarket.Web
3. Գործարկել նախագիծը (F5 Visual Studio-ում կամ dotnet run --project ArtMarket.Web): Հիմնական էջում ավտոմատ կբացվի Swagger UI-ը:

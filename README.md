<div align="center">

#  Gestion des Stagiaires

### Application web de gestion des stagiaires pour un organisme d'accueil

*Architecture classique — Back-office ASP.NET Core MVC + API · Front-office Bootstrap / jQuery / AJAX · Hébergement IIS*

<br/>

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC_%2B_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF_Core-Code_First-512BD4?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL_Server-LocalDB-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)

![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)
![jQuery](https://img.shields.io/badge/jQuery-AJAX-0769AD?style=for-the-badge&logo=jquery&logoColor=white)
![Identity](https://img.shields.io/badge/Auth-ASP.NET_Identity-2ea44f?style=for-the-badge)
![IIS](https://img.shields.io/badge/Serveur-IIS-0078D6?style=for-the-badge&logo=windows&logoColor=white)

</div>

---

##  Sommaire

- [Aperçu](#-aperçu)
- [Fonctionnalités](#-fonctionnalités)
- [Pile technique](#-pile-technique)
- [Architecture du projet](#-architecture-du-projet)
- [Prérequis](#-prérequis)
- [Démarrage rapide](#-démarrage-rapide)
- [Compte de démonstration](#-compte-de-démonstration)
- [Migrations Code First](#-migrations-code-first)
- [API REST](#-api-rest)
- [Déploiement sur IIS](#-déploiement-sur-iis)
- [Correspondance des exigences](#-correspondance-des-exigences)

---

##  Aperçu

Cette application permet à un organisme d'accueil de **gérer ses stagiaires** de bout en bout :
saisie, consultation, recherche, mise à jour et suppression, le tout derrière une
**authentification sécurisée**. Elle est bâtie sur une **architecture classique** en couches
(présentation → services → accès aux données) et respecte l'ensemble des bonnes pratiques
ASP.NET Core : `DbContext`, injection de dépendances, DTO / ViewModel, AutoMapper, validation
et pagination.

##  Fonctionnalités

| | Fonctionnalité | Détail |
|:--:|----------------|--------|
| | **Authentification** | Connexion, inscription, déconnexion, rôle *Administrateur* (ASP.NET Core Identity) |
| | **CRUD complet** | Créer, consulter, modifier et supprimer un stagiaire |
| | **Recherche & filtre** | Par nom, prénom, email, établissement + filtre par statut |
| | **Pagination** | Configurable (5 / 10 / 20 / 50 par page) |
| | **AJAX** | Suppression sans rechargement via l'API REST + notifications *toast* |
| | **Validation** | Côté serveur (DataAnnotations + `IValidatableObject`) **et** côté client (jQuery) |
| | **AutoMapper** | Mapping automatique *Entité ⇄ DTO / ViewModel* |
| | **Injection de dépendances** | Couche service abstraite (`IStagiaireService`) |

##  Pile technique

<div align="center">

| Couche | Technologie |
|--------|-------------|
| **Back-office** | ASP.NET Core 8 — MVC + API REST |
| **Front-office** | HTML5, Bootstrap 5, jQuery, AJAX |
| **Accès aux données** | Entity Framework Core (SQL Server) |
| **Authentification** | ASP.NET Core Identity |
| **Mapping** | AutoMapper |
| **Serveur** | IIS (hébergement in-process, `web.config` fourni) |

</div>

##  Architecture du projet

```
GestionStagiaires.sln
└── src/GestionStagiaires.Web/
    ├── Program.cs                          → configuration, DI, pipeline HTTP
    ├── appsettings.json                    → chaîne de connexion, compte admin (seed)
    ├── web.config                          → hébergement IIS (ASP.NET Core Module)
    │
    ├── Controllers/
    │   ├── HomeController.cs
    │   ├── AccountController.cs             → authentification
    │   ├── StagiairesController.cs          → back-office MVC (CRUD + vues)
    │   └── Api/StagiairesApiController.cs   → API REST
    │
    ├── Data/
    │   ├── ApplicationDbContext.cs          → DbContext (EF Core + Identity)
    │   ├── SeedData.cs                      → migration auto + données de démo
    │   └── Migrations/                      → migration Code First (InitialCreate)
    │
    ├── Models/
    │   ├── Entities/  → Stagiaire.cs, StatutStage.cs
    │   └── Identity/  → ApplicationUser.cs
    │
    ├── DTOs/          → StagiaireDto, StagiaireCreateDto, StagiaireUpdateDto
    ├── ViewModels/    → PagedResult<T>, StagiaireListViewModel, Login/Register
    ├── Services/      → IStagiaireService + StagiaireService
    ├── Mapping/       → MappingProfile.cs (profil AutoMapper)
    ├── Views/         → Razor (Home, Account, Stagiaires, Shared)
    └── wwwroot/       → css/site.css, js/site.js (helper AJAX)
```

##  Prérequis

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** ou **SQL Server LocalDB** (installé avec Visual Studio)
- *(Déploiement)* **IIS** + [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer)

##  Démarrage rapide

```bash
# 1. Restaurer les dépendances
dotnet restore

# 2. Lancer l'application
dotnet run --project src/GestionStagiaires.Web
```

Puis ouvrez  **https://localhost:7001**

> [!NOTE]
> Au **premier démarrage**, l'application applique automatiquement la **migration Code First**
> (`Database.Migrate()`), crée le **rôle + le compte administrateur**, puis insère quelques
> **stagiaires de démonstration**. Aucune commande manuelle n'est nécessaire.

La chaîne de connexion par défaut (`appsettings.json`) cible LocalDB :

```
Server=(localdb)\MSSQLLocalDB;Database=GestionStagiairesDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

## Compte de démonstration

| Email | Mot de passe |
|-------|--------------|
| `admin@stagiaires.local` | `Admin@123` |

> [!WARNING]
> Identifiants de démonstration — à modifier dans `appsettings.json` (section `SeedAdmin`) avant tout usage réel.

##  Migrations Code First

```bash
# Installer l'outil EF (une seule fois)
dotnet tool install --global dotnet-ef

# Créer une nouvelle migration après modification du modèle
dotnet ef migrations add NomDeLaMigration --project src/GestionStagiaires.Web

# Appliquer les migrations manuellement (optionnel : fait au démarrage)
dotnet ef database update --project src/GestionStagiaires.Web
```

##  API REST

Base : `/api/stagiaires` — authentification requise (cookie Identity).

| Méthode | Route | Description |
|:-------:|-------|-------------|
| `GET` | `/api/stagiaires?recherche=&statut=&page=1&pageSize=10` | Liste paginée |
| `GET` | `/api/stagiaires/{id}` | Détail d'un stagiaire |
| `POST` | `/api/stagiaires` | Création |
| `PUT` | `/api/stagiaires/{id}` | Mise à jour |
| `DELETE` | `/api/stagiaires/{id}` | Suppression |

Le front-office consomme cette API en **AJAX** (voir la suppression dans
`Views/Stagiaires/Index.cshtml` + `wwwroot/js/site.js`).

##  Déploiement sur IIS

```bash
# Publier l'application
dotnet publish src/GestionStagiaires.Web -c Release -o ./publish
```

1. Installer l'**ASP.NET Core Hosting Bundle** sur le serveur.
2. Créer un **site / une application** IIS pointant vers le dossier `publish`.
3. Pool d'applications en **« No Managed Code »** (exécution *in-process* via l'ASP.NET Core Module).
4. Ajuster la chaîne de connexion (`appsettings.json`) vers le SQL Server de production.

##  Correspondance des exigences

| Exigence | Implémentation |
|----------|----------------|
| Authentification | ASP.NET Core Identity — `AccountController`, `Program.cs` |
| Entity Framework | `ApplicationDbContext` + `EntityFrameworkCore.SqlServer` |
| Migration Code First | `Data/Migrations/…InitialCreate` |
| CRUD des stagiaires | `StagiairesController` (MVC) + `StagiairesApiController` (API) |
| `DbContext` | `Data/ApplicationDbContext.cs` |
| Services & injection de dépendances | `IStagiaireService` / `StagiaireService` |
| AutoMapper | `Mapping/MappingProfile.cs` |
| DTO / ViewModel | dossiers `DTOs/` et `ViewModels/` |
| Validation | DataAnnotations + `IValidatableObject` + jQuery Validation |
| Pagination | `PagedResult<T>`, `GetPagedAsync`, vue `Index` |
| Front-office HTML5 / Bootstrap / jQuery / AJAX | `Views/`, `wwwroot/` |
| Serveur IIS | `web.config` + section déploiement |

<div align="center">

---

**Développé dans le cadre d'un stage** · ASP.NET Core 8 · Entity Framework Core · Bootstrap 5

</div>

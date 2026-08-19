# Plateforme de gestion des stages — POC ASP.NET Core MVC

Mini-projet développé dans le cadre du stage de découverte à l'ONEE (Direction
des Systèmes d'Information), destiné à démontrer la maîtrise pratique des
concepts fondamentaux de la POO en C# et du framework ASP.NET Core MVC.

Ce POC constitue le squelette technique de la plateforme complète décrite
dans le cahier des charges fonctionnel.

---

## 1. Stack technique

- **.NET 8** (ASP.NET Core MVC)
- **Entity Framework Core 8** (Code First, SQL Server LocalDB)
- **Razor** pour les vues
- **Bootstrap 5** pour l'UI

---

## 2. Concepts illustrés

### Programmation Orientée Objet
- Classes, propriétés, encapsulation (`DemandeStage`)
- Énumérations (`EtatDemande`, `TypeStage`)
- Propriétés calculées (`DureeEnJours`)
- Méthodes métier avec transitions d'état contrôlées (`Soumettre()`)
- Data Annotations pour la validation déclarative

### ASP.NET Core MVC
- Architecture Modèle / Vue / Contrôleur
- Routing conventionnel
- Injection de dépendances (DbContext dans le contrôleur)
- Model binding et validation côté serveur + client
- Vues Razor fortement typées, layouts partagés, partial views
- Tag Helpers (`asp-for`, `asp-controller`, `asp-action`)

### Entity Framework Core
- Approche Code First
- DbContext personnalisé (`ApplicationDbContext`)
- Requêtes LINQ to Entities (recherche, filtrage, tri)
- Méthodes asynchrones (`async` / `await`)
- Seed de données via `HasData()`

---

## 3. Fonctionnalités

- **CRUD complet** sur les demandes de stage (Create, Read, Update, Delete)
- **Recherche** par sujet, stagiaire ou entreprise
- **Filtrage** par état du workflow
- **Workflow d'états** : Brouillon → Soumise → Validée encadrant → Validée RH →
  En cours → Terminée → Clôturée (avec possibilité de rejet)
- **Validation** des champs (obligatoires, longueurs, format email, cohérence
  des dates)
- **Interface responsive** avec Bootstrap 5

---

## 4. Structure du projet

```
StagesPlatform/
├── Program.cs                          # Point d'entrée, config DI et pipeline
├── appsettings.json                    # Chaîne de connexion SQL Server
├── StagesPlatform.csproj               # Dépendances NuGet
├── Models/
│   ├── DemandeStage.cs                 # Entité principale
│   ├── EtatDemande.cs                  # Enum workflow
│   ├── TypeStage.cs                    # Enum type
│   └── ErrorViewModel.cs
├── Data/
│   └── ApplicationDbContext.cs         # DbContext EF Core + seed
├── Controllers/
│   ├── HomeController.cs
│   └── DemandesStageController.cs      # CRUD complet
├── Views/
│   ├── _ViewImports.cshtml
│   ├── _ViewStart.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   └── DemandesStage/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       ├── Edit.cshtml
│       ├── Details.cshtml
│       └── Delete.cshtml
└── wwwroot/
    └── css/site.css
```

---

## 5. Installation et exécution

### Prérequis
- **.NET 8 SDK** — [téléchargement](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server LocalDB** (livré avec Visual Studio) ou SQL Server Express
- **Visual Studio 2022** (recommandé) ou VS Code avec l'extension C#

### Étapes

```bash
# 1. Cloner ou extraire le projet
cd StagesPlatform

# 2. Restaurer les dépendances NuGet
dotnet restore

# 3. Lancer l'application
dotnet run
```

L'application ouvre automatiquement `https://localhost:5001` (ou le port
configuré). La base de données est créée automatiquement au premier lancement
avec deux demandes de test.

### Sous Visual Studio
- Ouvrir `StagesPlatform.csproj`
- Appuyer sur **F5** pour lancer en mode debug

---

## 6. Captures d'écran suggérées pour la démonstration

- Page d'accueil
- Liste des demandes avec filtre
- Formulaire de création (avec validation)
- Détails d'une demande + changement d'état
- Confirmation de suppression

---

## 7. Extensions prévues (roadmap)

- Authentification et rôles via **ASP.NET Core Identity**
- Upload des livrables (CV, convention, rapports)
- Module d'analyse et scoring des CV via un service IA (Python / API REST)
- Notifications email
- Tableau de bord et export statistique

---

**Auteur.** Ahmed Zhiri — Stagiaire ONEE / DSI

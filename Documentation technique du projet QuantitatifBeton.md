# Documentation Technique : Portail IDES - QuantitatifBéton

**Auteur**: Manus AI & mg.IDES
**Date**: 19 Janvier 2026
**Version**: 1.1 (Fusionnée)

---

## 1. Introduction & Vision Métier

**QuantitatifBéton** est une application de bureau Windows conçue pour les professionnels du bâtiment et du génie civil. Elle permet de créer des métrés quantitatifs détaillés pour des ouvrages en béton armé. L'application vise à remplacer les tableurs Excel par une solution structurée, dynamique et centralisée, permettant de :

1.  **Standardiser le chiffrage** en utilisant un catalogue d'éléments de construction prédéfinis.
2.  **Automatiser les calculs** de quantités (volume de béton, poids d'aciers, surface de coffrage) via un système de formules.
3.  **Accélérer la création de métrés** en permettant l'ajout rapide d'éléments et la saisie de leurs dimensions.
4.  **Garantir la cohérence** en centralisant la définition des propriétés et des méthodes de calcul.

Le public cible est constitué d'ingénieurs structure, d'économistes de la construction et de techniciens de bureau d'études.

Construite sur la plateforme Windows Presentation Foundation (WPF) avec le framework .NET, l'application adopte une architecture moderne et robuste pour garantir la maintenabilité, l'évolutivité et la réactivité de l'interface utilisateur.

Ce document fournit une description technique complète du projet, destinée à des développeurs experts souhaitant comprendre, maintenir, ou faire évoluer l'application, notamment en vue d'une migration vers d'autres technologies comme Blazor.

---

## 2. Architecture Générale

L'application est fondée sur le patron de conception **MVVM (Model-View-ViewModel)**, un standard dans l'écosystème WPF qui favorise une séparation nette des préoccupations.

### 2.1. Composants Principaux

*   **Modèles (Models)** : Représentent les objets métier et les structures de données. Ils sont agnostiques de l'interface utilisateur et encapsulent la logique et les données fondamentales de l'application (ex: `ElementDynamique`, `DefinitionElement`). C'est la partie la plus importante et la plus facilement portable.

*   **Vues (Views)** : Définissent la structure, l'apparence et le comportement de l'interface utilisateur. Écrites en XAML, elles sont "stupides" et se contentent de se lier (*data binding*) aux propriétés et commandes exposées par les ViewModels.

*   **ViewModels** : Agissent comme un pont entre les Modèles et les Vues. Ils exposent les données des Modèles sous une forme facilement consommable par les Vues (via des propriétés notifiant les changements) et contiennent la logique de présentation et les commandes (actions utilisateur).

### 2.2. Technologies et Bibliothèques Clés

| Bibliothèque                  | Version (indicative) | Rôle                                                                                        |
| ----------------------------- | -------------------- | ------------------------------------------------------------------------------------------- |
| **.NET**                      | 8.0+                 | Framework principal d'exécution.                                                            |
| **WPF**                       | -                    | Framework UI pour les applications de bureau Windows.                                       |
| **CommunityToolkit.Mvvm**     | 8.2.0+               | Implémentation MVVM de référence (`ObservableObject`, `RelayCommand`, `IMessenger`).          |
| **Entity Framework Core**     | 8.0+                 | ORM pour l'accès à la base de données du catalogue (avec le provider **SQLite**).             |
| **AvalonEdit**                | 6.3.0+               | Contrôle d'édition de texte avancé, utilisé pour l'éditeur de formules avec coloration syntaxique. |
| **NCalc**                     | 2.2.0+               | Bibliothèque d'évaluation d'expressions mathématiques, utilisée comme moteur de calcul.       |
| **Microsoft.Xaml.Behaviors.Wpf**| 1.1.39+              | Permet d'ajouter des comportements interactifs (`AutoCompleteBehavior`) aux éléments XAML. |

### 2.3. Structure du Projet

```Plain Text
/QuantitatifBeton
├── Data/               # Contexte de la base de données (DbContext) et initialisation.
├── Models/             # Classes métier principales.
│   ├── Catalogue/      # Modèles liés à la définition des éléments (le catalogue).
│   └── Proprietes/     # Modèles des différents types de propriétés (Double, String, Formule...).
├── Services/           # Logique applicative (calcul, création d'objets, dialogues).
├── ViewModels/         # Contient tous les ViewModels de l'application.
├── Views/              # Fenêtres et UserControls (fichiers .xaml et .xaml.cs).
├── Controls/           # Contrôles UI personnalisés (ex: éditeur de formules).
├── Converters/         # Convertisseurs de valeurs pour le data binding.
├── Resources/          # Icônes, styles globaux, etc.
├── Migrations/         # Fichiers de migration générés par EF Core.
└── App.xaml.cs         # Point d'entrée de l'application, où l'injection de dépendances est initialisée.
```

## 3. Modèle de Données Détaillé (La "Base de Connaissance")

C'est le cœur de l'application. Il est divisé en deux domaines : le **Catalogue** (la définition des "types") et le **Métré** (les instances concrètes).

### 3.1. Le Catalogue (Entités EF Core)

Ces classes définissent les "briques de Lego" disponibles pour la construction. La base de données `catalogue.db` est gérée par EF Core.

#### `DefinitionElement`
Représente un type d'élément constructible (ex: "Poutre BA", "Dalle 20cm", "Fondation Fût+Massif").

```csharp
public class DefinitionElement
{
    public int Id { get; set; }
    public string Nom { get; set; } // "Poutre BA"
    public string Categorie { get; set; } // "Poutres", "Dalles"
    public string PrefixeRepere { get; set; } // "P-", "D-"
    public string DesignationTemplate { get; set; } // "Poutre {Largeur}x{Hauteur}"

    // --- Gestion des éléments composites ---
    public bool EstUnOuvrage { get; set; } // true si c'est un groupe (ex: "Fondation complète")
    public int? ParentId { get; set; }
    public virtual DefinitionElement? Parent { get; set; }
    public virtual ICollection<DefinitionElement> Enfants { get; set; } = new List<DefinitionElement>();

    // --- Relation vers les propriétés ---
    public virtual ICollection<ElementPropriete> ProprietesAssociees { get; set; } = new List<ElementPropriete>();
}
```

#### `DefinitionPropriete`
Représente la définition d'une caractéristique (ex: "Largeur", "Hauteur", "Ratio Acier").

```csharp
public class DefinitionPropriete
{
    public int Id { get; set; }
    public string Nom { get; set; } // Nom interne, sans espaces (ex: "Largeur")
    public string NomAffichage { get; set; } // Nom pour l'UI (ex: "Largeur (m)")
    public string TypeDonnee { get; set; } // "double", "string", "bool", "formule"
    public string? Unite { get; set; } // "m", "kg/m³"
    public string Categorie { get; set; } // "Dimensions", "Acier"

    public virtual ICollection<ElementPropriete> ElementsAssocies { get; set; } = new List<ElementPropriete>();
}
```
#### `ElementPropriete (Table de Liaison)`
C'est l'entité cruciale qui connecte un DefinitionElement à une DefinitionPropriete. C'est elle qui stocke la valeur ou la formule par défaut spécifique à cette association.

```csharp
public class ElementPropriete
{
    public int Id { get; set; }
    public int DefinitionElementId { get; set; }
    public virtual DefinitionElement DefinitionElement { get; set; }
    public int DefinitionProprieteId { get; set; }
    public virtual DefinitionPropriete DefinitionPropriete { get; set; }

    // La valeur/formule pour CETTE propriété sur CET élément.
    // Ex: Pour "Poutre BA", la valeur de "Largeur" est "0.20".
    // Ex: Pour "Poutre BA", la formule de "VolumeBeton" est "{Largeur}*{Hauteur}".
    public string? Valeur { get; set; }
}
```

### 3.2. Le Métré (Modèles en Mémoire)
Ces classes représentent les objets concrets manipulés par l'utilisateur dans la grille de métré.
#### `ElementDynamique`
Représente une ligne dans le métré (ex: la poutre "P-101"). Il est créé à partir d'une DefinitionElement du catalogue.

```csharp
public partial class ElementDynamique : ObservableObject
{
    // Propriétés de base
    public string NomElement { get; set; }
    public string Repere { get; set; }
    public string DesignationTemplate { get; set; }

    // Collection des instances de propriétés avec leurs valeurs saisies par l'utilisateur
    public ObservableCollection<Propriete> Proprietes { get; } = new();

    // --- Logique de Calcul ---
    private readonly ElementCalculator _calculator;

    // Raccourcis pour les colonnes de la grille de métré.
    // Ces propriétés appellent le calculateur.
    public object? VolumeBeton => _calculator.GetValue("VolumeBeton");
    public object? TotalAcierHA => _calculator.GetValue("PoidsHA");
    // ... etc. pour PoidsTS, SurfaceCoffrage, AvantMetre...
}
```
#### `Propriete (et ses dérivés)`
Classe de base abstraite pour une instance de propriété dans un ElementDynamique.
```csharp
public abstract class Propriete : ObservableObject
{
    public string Nom { get; } // "Largeur"
    public string NomAffichage { get; } // "Largeur (m)"
    public string? Unite { get; set; }
    public abstract object? ValeurObjet { get; set; }
}
```
#### `Classes dérivées :`
```csharp
ProprieteDouble : public double Valeur { get; set; }
ProprieteString : public string? Valeur { get; set; }
ProprieteBool : public bool Valeur { get; set; }
ProprieteFormule : public string? Formule { get; set; } (ne stocke pas de valeur, seulement la formule à évaluer).
```
## 4. Fonctionnalités et Processus Clés
### 4.1. Gestion du Catalogue
* **Vue principale (`GestionCatalogueView`)** : Affiche une `TreeView` des `DefinitionElement` groupés par `Categorie`. Permet de sélectionner, créer ou supprimer un élément.
* **Vue d'édition (`EditeurElementView`)** :
    * Permet de modifier les informations de base d'une `DefinitionElement` (Nom, Catégorie, etc.).
    * Contient une interface à double liste pour assigner des `DefinitionPropriete` à l'élément.
    * La liste de droite ("Propriétés assignées") est une `DataGrid` qui permet de saisir la valeur par défaut ou la formule pour chaque propriété (stockée dans l'entité `ElementPropriete`).
    * Utilise un `AvalonTokenEditor` pour l'édition des formules, avec autocomplétion des jetons `{Propriete}`.
* **Gestion des Propriétés (`GestionProprietesView`)** : Une fenêtre CRUD complète pour gérer la liste globale de toutes les `DefinitionPropriete` disponibles dans l'application. La suppression est bloquée si une propriété est utilisée par au moins un `DefinitionElement`.

## 4.2. Création d'un ElementDynamique (Factory Pattern)
Le `ElementFactory` est une classe statique centrale avec une méthode `CreerElementDynamique`(`DefinitionElement definition`). Son rôle est de :
1. Prendre une `DefinitionElement` en entrée.
2. Créer une nouvelle instance de `ElementDynamique`.
3. Parcourir les `ProprietesAssociees` de la définition.
4. Pour chaque association, instancier la classe `Propriete` correspondante (`ProprieteDouble`, `ProprieteFormule`, etc.).
5. Assigner la Valeur de l'association (`ElementPropriete.Valeur`) comme valeur initiale de l'instance de `Propriete`.
6. Retourner l'`ElementDynamique` complet, prêt à être utilisé.

## 4.3. Moteur de Calcul
Le calcul des quantités est géré par une interaction entre `ElementDynamique` et un service de calcul.
* **`ElementCalculator` (associé à chaque `ElementDynamique`)** :
    * Possède une méthode `GetValue`(`string nomPropriete`).
    * Implémente un cache pour ne pas recalculer les mêmes valeurs plusieurs fois.
    * Quand une propriété de base (ex: `Largeur`) change, le cache est invalidé (`InvalidateCache()`).
    * Quand `GetValue` est appelé, il cherche la formule correspondante dans l' `ElementDynamique` et la passe au `MoteurCalculService`.
* **`MoteurCalculService` (statique)** :
    * Utilise la bibliothèque `NCalc` pour évaluer des expressions mathématiques textuelles.
    * Sa méthode `Evaluer(string formule, ElementDynamique element)` prend une formule et le contexte.
    * Elle prépare une `Expression` NCalc.
    * Pour chaque paramètre requis par la formule (ex: `{Largeur}`), elle demande sa valeur à l' `ElementDynamique` (ce qui peut déclencher un appel récursif pour calculer une dépendance).
    * Elle retourne le résultat final.

## 4.4. Éléments Composites (Ouvrages)
* **Modèle** : La DefinitionElement a une relation parent-enfant sur elle-même et un booléen `EstUnOuvrage`.
* **Catalogue** : L'éditeur de catalogue permet de définir un élément comme "ouvrage" et de lui assigner une liste de `DefinitionElement` enfants.
* **Logique d'ajout au métré** : Quand l'utilisateur ajoute un "ouvrage" au métré, l'application ne crée pas une ligne pour l'ouvrage lui-même. À la place, elle parcourt sa collection Enfants et exécute la logique d'ajout pour chacun des enfants, créant ainsi plusieurs lignes d'un coup.

# 5. Démarrage et Compilation
1. **Prérequis** : Visual Studio 2022 (ou plus récent) avec la charge de travail ".NET desktop development", et le SDK .NET 8+.
2. **Restauration des packages** : Visual Studio restaurera automatiquement les packages NuGet au premier build.
3. **Base de données** : Au premier lancement, Entity Framework Core créera automatiquement le fichier catalogue.db (par exemple dans %LOCALAPPDATA%/QuantitatifBeton) et appliquera les migrations pour créer le schéma. Le `CatalogueDbInitializer` peut peupler la base avec des données initiales.
4. **Exécution** : Lancer le projet en mode Debug (F5) depuis Visual Studio.

# 6. Transposition vers Blazor
Recréer ce projet en Blazor est une évolution naturelle.
* **Modèles (Model) & Services** : 100% réutilisables. Les classes `DefinitionElement`, `Propriete`, `ElementDynamique`, `MoteurCalculService`, etc., n'ont aucune dépendance WPF. Ils peuvent être placés dans une bibliothèque de classes .NET Standard.
* **Base de Données** : EF Core et SQLite sont compatibles avec Blazor Server. Pour Blazor WebAssembly, une API web sera nécessaire pour accéder à la base de données.
* **ViewModels** : Le concept est remplacé par le modèle de composant Blazor. La logique du ViewModel (commandes, propriétés) sera déplacée dans le bloc `@code` des composants Razor. Le `State Management` devient un enjeu clé.
* **Vues (View)** : Le XAML est remplacé par des composants Razor (.razor).
    * `DataGrid` -> Un composant de grille comme ceux de `MudBlazor`, `Blazorise`, `Radzen`.
    * `AvalonTokenEditor` -> C'est le plus gros défi. Il faudra trouver un éditeur de texte basé sur JavaScript (comme `Monaco Editor`) et créer un wrapper Blazor pour l'intégrer via interop JS.
* **Services (DialogService)** : Le concept est identique. Ils seront enregistrés dans le conteneur d'injection de dépendances de Blazor (`Program.cs`) et injectés dans les composants avec `@inject`.

# 7. Conclusion et Axes d'Amélioration
QuantitatifBéton est une application WPF bien architecturée qui tire parti des meilleures pratiques modernes de l'écosystème .NET. La séparation claire en MVVM, l'utilisation d'un ORM comme EF Core, et l'intégration d'un moteur de calcul flexible en font une base solide et maintenable.
* Axes d'amélioration potentiels :
    * Finaliser la sérialisation des documents de métré (.qba).
    * Ajouter des tests unitaires pour valider la logique du moteur de calcul.
    * Permettre l'export du métré vers des formats comme Excel ou PDF.
    * Améliorer l'UI/UX avec des validations d'entrée plus robustes.
    * Optimiser les performances sur de très grands documents (virtualisation).
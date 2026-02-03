# \# Unity Code Sample – Systems \& Tooling

# 

# This folder contains a curated selection of C# scripts from a larger Unity project.

# The purpose of this sample is to demonstrate \*\*software architecture, system design,

# and maintainable Unity development practices\*\*, rather than a complete or polished game.

# 

# These files were selected to highlight how I approach modular systems, data-driven

# logic, UI integration, and extensible gameplay/simulation frameworks in Unity.

# 

# ---

# 

# \## Recommended Entry Points

# 

# If you are short on time, start with the following files:

# 

# 1\. \*\*StatsComponent.cs\*\*  

# &nbsp;  A data-driven stat system supporting modifiers, caching, and runtime updates.

# &nbsp;  Designed for flexibility, clarity, and scalability.

# 

# 2\. \*\*MechBrain.cs\*\*  

# &nbsp;  An AI controller coordinating decision-making, targeting, and action execution.

# &nbsp;  Demonstrates system orchestration and separation of responsibilities.

# 

# 3\. \*\*BaseHealthBar.cs\*\*  

# &nbsp;  A UI controller showing clean separation between data, presentation, and animation

# &nbsp;  logic (including delayed “chip” effects).

# 

# ---

# 

# \## Folder Overview

# 

# \### Stats/

# Core stat and modifier architecture used across multiple systems.

# 

# \- `StatsComponent.cs` – Central stat registry and access layer  

# \- `StatInstance.cs` – Runtime stat value with modifier aggregation  

# \- `StatModifier.cs` – Additive and multiplicative modifiers  

# \- `StatTypes.cs` – Enum-based stat definitions  

# 

# \*\*Concepts demonstrated:\*\*

# \- Data-driven design

# \- Encapsulation and OOP

# \- Runtime-safe access patterns

# 

# ---

# 

# \### AI/

# ScriptableObject-driven AI architecture for tactical decision-making.

# 

# \- `AIAction.cs` – Abstract action definition  

# \- `AIProfile.cs` – Data-driven behavior configuration  

# \- `Objective.cs` – Goal-oriented targeting logic  

# \- `EnemyCombatDirector.cs` – High-level AI coordination  

# \- `MechBrain.cs` – AI execution and system integration  

# 

# \*\*Concepts demonstrated:\*\*

# \- Separation of data and behavior

# \- Extensible decision systems

# \- Clean orchestration logic

# 

# ---

# 

# \### UI/

# User interface logic focused on clarity, responsiveness, and maintainability.

# 

# \- `BaseHealthBar.cs` – Health visualization with delayed feedback

# \- `DamagePopup.cs` / `DamagePopupManager.cs` – Pooled floating damage indicators

# 

# \*\*Concepts demonstrated:\*\*

# \- Presentation-layer isolation

# \- Pooling and lifecycle management

# \- UX-focused feedback systems

# 

# ---

# 

# \### Utilities/

# Reusable helper components.

# 

# \- `FollowTarget.cs` – Configurable transform-follow behavior with axis control

# 

# \*\*Concepts demonstrated:\*\*

# \- Reusable component design

# \- Clear configuration via inspector

# 

# ---

# 

# \## Technical Notes

# 

# \- Language: \*\*C#\*\*

# \- Engine: \*\*Unity\*\*

# \- Architecture emphasis: modular systems, clarity, extensibility

# \- This is a subset of a larger project; unrelated gameplay, assets, and prototypes

# &nbsp; have been intentionally excluded.

# 

# ---

# 

# \## About This Sample

# 

# This code is intended to reflect how I write production-quality Unity systems for

# interactive applications, simulations, and tools. It prioritizes readability,

# maintainability, and clear data flow over visual polish or content completeness.


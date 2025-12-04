# Pico VR Unity Project Setup Guide

## Project Overview
This Unity project is configured for Pico VR development using Unity 2022.3.16f1c1. The project includes all previous work and is ready for Pico SDK integration.

## Prerequisites

### Required Software
- **Unity Hub** installed on your system
- **Unity 2022.3.16f1c1** installed via Unity Hub
- **Pico SDK for Unity** (to be uploaded separately)

## Setup Instructions

### 1. Create New Unity Project
1. Open Unity Hub
2. Click **New Project**
3. Select **3D Core** template
5. Ensure Unity Version is set to **2022.3.16f1c1**
6. Click **Create Project**

### 2. Import Project Package
After creating the project:
1. In Unity Editor, go to **Assets → Import Package → Custom Package**
2. Select the provided `.unitypackage` file containing all previous work
3. Click **Import** and ensure all assets are selected
4. Wait for import to complete

### 3. Install Pico SDK
**Note:** Pico SDK will be provided separately. Once you receive it:
In the Unity Editor, go to **Window → Package Manager → add package from disk**
select the `package.json`file and it will automatically set up the sdk.

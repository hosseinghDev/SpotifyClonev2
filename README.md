# 🎶 SpotifyClonev2

SpotifyClonev2 is a cross-platform **music player application** built entirely with the **.NET ecosystem**.  
It features a **native mobile client** developed with **.NET MAUI** and is powered by a robust **backend API** built with **ASP.NET Core**.

---

## 🎵 About The Project

This project is a demonstration of building a **full-stack, client-server application** using the latest Microsoft technologies.  

It provides a **functional and visually appealing music streaming app** where users can:
- Browse their music library  
- Play and control songs  
- Manage playlists  

All from an **Android device**.

---

## ✨ Key Features

- **Cross-Platform Client**: Built with .NET MAUI (Android-ready, extendable to iOS, Windows, macOS).  
- **Dynamic Music Library**: Browse/search songs and artists from the backend.  
- **Full-Featured Player**:
  - Play / Pause / Next / Previous  
  - Shuffle & Repeat modes  
  - Seekable progress bar  
- **Interactive Song List**: Like songs, add to queue, manage from the home screen.  
- **Backend-Powered**: Metadata + audio streaming served via ASP.NET Core API.  
- **Bottom Navigation**: Easy navigation between *Home, Library, Upload*.  

---

## 🛠️ Technology Stack

### Frontend Client – `SpotifyClone.Maui`
- **Framework**: .NET MAUI  
- **Language**: C#  
- **UI Markup**: XAML  
- **Pattern**: MVVM (Model-View-ViewModel)  
- **Libraries**:  
  - `CommunityToolkit.Mvvm` – MVVM helpers  
  - `.NET MAUI MediaElement` – native audio playback  

### Backend Server – `SpotifyClone.Api`
- **Framework**: ASP.NET Core Web API  
- **Language**: C#  
- **Database**: Entity Framework Core (with SQL Server/SQLite/etc.)  
- **API Docs**: Swagger (Swashbuckle)  

---

## 🚀 Getting Started

Follow these steps to run the project locally.

### ✅ Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/en-us/download) (7 or 8 recommended)  
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with:  
  - .NET MAUI workload  
  - ASP.NET & Web Development workload  
- Android Emulator or physical Android device  
- A database server (e.g., SQL Server Express)  

---

## 1️⃣ Backend Setup (`SpotifyClone.Api`)

**Clone the repository:**
```bash
git clone https://github.com/hosseinghDev/SpotifyClonev2.git
cd SpotifyClonev2
```

**Configure the database:**

- Open the SpotifyClone.Api project
- In appsettings.Development.json, update the ConnectionStrings section with your database credentials

**Apply database migrations:**
```bash
cd SpotifyClone.Api
dotnet ef database update
```

**Run the backend server:**
```bash
dotnet run --project SpotifyClone.Api
```
---

## 2️⃣ Frontend Setup (SpotifyClone.Maui)

**Open the solution:**

- Open the solution file SpotifyClone.sln in Visual Studio

**Configure the API endpoint:**

- Locate the file in SpotifyClone.Maui where the API base URL is defined
- Update it to point to your backend server

**Run the application:**

- Set SpotifyClone.Maui as the startup project
- Select your target (Android Emulator or physical device)
- Press F5 to build and deploy

---
## 🤝 Contributing
Contributions make the open-source community amazing! 🎉
If you'd like to contribute:

1- Fork the project
2- Create your feature branch
3- Commit your changes
4- Push to the branch
5- Open a Pull Request

## ⭐ Show your support
Give a ⭐️ if this project helped you!

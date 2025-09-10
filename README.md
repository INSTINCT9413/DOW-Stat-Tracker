

# Dawn of War Stat Tracker 🎮📊

![License](https://img.shields.io/github/license/yourusername/DOW-Stat-Tracker)
![Stars](https://img.shields.io/github/stars/yourusername/DOW-Stat-Tracker?style=social)
![Build](https://img.shields.io/badge/build-passing-brightgreen)

A **desktop application** for tracking **Dawn of War: Soulstorm (DE)** multiplayer stats.  
Built in **C# WinForms**, this app fetches data from the official Relic API and presents it in a clean, modern UI with advanced features like race-specific stats, favorite race detection, and leaderboard tracking.

---

## ✨ Features

- 🔎 **Search Players by Alias**  
- 📈 **Display Recent Matches (Automatched only)**  
- 🏆 **Favorite Race Detection** (based on games played)  
- ⚔️ **Per-Race Statistics** (wins, losses, last match date)  
- 📊 **Leaderboard Integration** (rankings, XP, DOW-themed rank system)  
- 🖼️ **Steam Profile Picture Display** (auto-fetch from Steam, fallback image included)  
- ⏱️ **Auto-Refresh Stats** (configurable interval in settings)  
- 💾 **Persistent User Settings** (remembers last searched alias)  
- 📂 **Quick Access to Settings File** (open `user.config` directly in Explorer)  
- 🎨 **Custom Rank System** based on XP with WH40k-inspired titles (Recruit → Emperor)  
- 🔄 **Loading Overlay Animation** while fetching data  

---

## 📷 Screenshots

> Replace these with actual screenshots later:

![Main UI](screenshots/main_ui.png)  
![Leaderboard](screenshots/leaderboard.png)  
![Race Stats](screenshots/race_stats.png)

---

## 🚀 Installation

1. Download the latest release from the [Releases](https://github.com/yourusername/DOW-Stat-Tracker/releases) page.  
2. Extract the `.zip` file.  
3. Run `DOW_Stat_Tracker.exe`.  

---

## ⚙️ Usage

1. Enter a **player alias** and click **Get Stats**.  
2. Navigate through tabs:  
   - **Overview** → Favorite race, best win rate, top rating.  
   - **Recent Matches** → Automatched games list.  
   - **Race Tabs** → Detailed per-race stats.  
   - **Leaderboard** → Global ranking with XP progress bars.  
3. Configure auto-refresh and preferences in **Settings**.

---

## 🛡️ Rank System

The app includes a **custom WH40k-themed rank system** based on player XP:  

| XP Required | Rank Name             |
|-------------|-----------------------|
| 0           | Recruit               |
| 10,000      | Scout                 |
| 50,000      | Veteran Scout         |
| 150,000     | Trooper               |
| 300,000     | Stormtrooper          |
| 600,000     | Veteran Stormtrooper  |
| 1,000,000   | Sergeant              |
| 1,500,000   | Lieutenant            |
| 2,200,000   | Captain               |
| 3,600,000   | Major                 |
| 4,200,000   | Chapter Master        |
| 6,000,000   | Lord Commander        |
| 8,000,000   | Inquisitor            |
| 9,700,000   | Emperor               |

---

## 🛠️ Tech Stack

- **Language:** C# (.NET 5+ / WinForms)  
- **UI:** Windows Forms + Custom Controls  
- **Data:** [Relic Dawn of War API](https://dow-api.reliclink.com/)  
- **Serialization:** Newtonsoft.Json  

---

## 📌 Roadmap

- [ ] Add support for **team games (2v2, 3v3, 4v4)**  
- [ ] Export stats to **CSV/Excel**  
- [ ] More detailed **match history breakdowns**  
- [ ] Improved **UI theming**  

---

## 🤝 Contributing

Contributions are welcome!  
Please open an [issue](https://github.com/yourusername/DOW-Stat-Tracker/issues) or submit a pull request.  

---

## 📜 License

This project is licensed under the **MIT License**.  
See [LICENSE](LICENSE) for details.  

---

## 🎖️ Credits

- Relic Entertainment for the **Dawn of War API**  
- WH40k community for inspiration  
- All contributors & testers ❤️  

---

## ⭐ Support

If you like this project, please **star the repo** ⭐ and share it with fellow commanders!

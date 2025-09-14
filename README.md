

# Dawn of War Stat Tracker 🎮📊

[![EULA](https://img.shields.io/badge/license-Custom%20EULA-blue)](./EULA.txt)
![Stars](https://img.shields.io/github/stars/INSTINCT9413/DOW-Stat-Tracker?style=social)
![Build](https://img.shields.io/badge/build-passing-brightgreen)

A **desktop application** for tracking [**Warhammer 40,000: Dawn of War - Definitive Edition**](https://store.steampowered.com/app/3556750/Warhammer_40000_Dawn_of_War__Definitive_Edition/) multiplayer stats.  
Built in **C# WinForms**, this app fetches data from the official Relic API and presents it in a clean, modern UI with advanced features like race-specific stats, favorite race detection, and leaderboard tracking.

---

## ✨ Features  

- 🔎 **Search Players by Alias** – quickly find and highlight players in any leaderboard  
- 📈 **Display Recent Matches** – shows automatched matches with stats  
- 🏆 **Favorite Race Detection** – determines your most-played race by total games  
- ⚔️ **Per-Race Statistics** – wins, losses, streaks, and last match date per race  
- 📊 **Leaderboard Integration** – supports 1v1, 2v2, 3v3, and more, ranked by official API rating  
- 🎖️ **Custom WH40k Rank System** – XP-based ranks (Recruit → Emperor) with progress tracking  
- 📊 **In-Cell Progress Bars** – visualize XP to next rank and XP to next player directly in the leaderboard grid  
- 🔄 **Cross-Leaderboard Top 100** – combines all modes (1v1, 2v2, 3v3, etc.) and ranks by rating  
- 🖼️ **Steam Profile Picture Display** – auto-fetch from Steam, with fallback image included  
- ⏱️ **Auto-Refresh Stats** – configurable refresh interval in settings  
- 💾 **Persistent User Settings** – remembers last searched alias and preferences  
- 📂 **Quick Access to Settings File** – opens `user.config` directly in Explorer  
- 🔄 **Loading Overlay Animation** – smooth visual feedback while fetching leaderboard data  

---

## 🚀 Installation

1. Download the latest release from the [Releases](https://github.com/INSTINCT9413/DOW-Stat-Tracker/releases) page.  
2. Extract the `.zip` file.  
3. Run `DOW_Stat_Tracker.exe`.  

---

## ⚙️ Usage

1. Enter a **player alias** and click **Search Player**.  
2. Navigate through tabs:  
   - **Profile Summary** → Favorite race, best win rate, top rating.  
   - **Recent Matches** → Automatched games list.  
   - **Race Stats** → Detailed per-race stats.  
   - **Community** → Contaions all global ranking with XP progress bars.  
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
Please open an [issue](https://github.com/INSTINCT9413/DOW-Stat-Tracker/issues) or submit a pull request.  

---

## 📜 License

This project is distributed under a custom End User License Agreement.  
Please see the full [EULA here](./EULA.txt).


---

## 🎖️ Credits

- Relic Entertainment for the **Dawn of War API**  
- WH40k community for inspiration  
- All contributors & testers ❤️  

---

## ⭐ Support

If you like this project, please **star the repo** ⭐ and share it with fellow commanders!

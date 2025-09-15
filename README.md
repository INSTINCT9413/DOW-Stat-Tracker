

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

The app includes a **custom WH40k-themed rank system** based on player XP, scaling from *Recruit* all the way to *Emperor of Mankind* (20,000,000 XP).  

| Level | XP Required | Rank Name               |
|-------|-------------|-------------------------|
| 0     | 0           | Recruit                 |
| 1     | 500         | Conscript               |
| 2     | 750         | Guardsman               |
| 3     | 1125        | Corporal                |
| 4     | 1687        | Sergeant                |
| 5     | 2531        | Battle Brother          |
| 6     | 3796        | Lieutenant              |
| 7     | 5694        | Tank Commander          |
| 8     | 8541        | Captain                 |
| 9     | 12812       | Major                   |
| 10    | 19218       | Colonel                 |
| 11    | 28827       | Commissar               |
| 12    | 43241       | Colonel Commissar       |
| 13    | 64861       | Castellan               |
| 14    | 97291       | Codicier                |
| 15    | 145936      | Librarian               |
| 16    | 218904      | Chief Librarian         |
| 17    | 328356      | Chapter Master          |
| 18    | 492534      | Major General           |
| 19    | 738801      | Lieutenant General      |
| 20    | 1108201     | Marshal                 |
| 21    | 1328679     | General                 |
| 22    | 1592863     | Inquisitor              |
| 23    | 1909111     | Inquisitor Lord         |
| 24    | 2288362     | Master                  |
| 25    | 2740925     | Grandmaster             |
| 26    | 3281084     | Lord General            |
| 27    | 3926263     | Lord General Militant   |
| 28    | 4704882     | Warmaster               |
| 29    | 5633985     | Lord Commander          |
| 30    | 6749468     | Lord Commander Militant |
| 31    | 8087713     | Commandant              |
| 32    | 9686716     | Lord Constable          |
| 33    | 10601427    | Captain-General         |
| 34    | 12899419    | Chief Commandant        |
| 35    | 14644580    | Primarch                |
| 36    | 16644580    | Imperial Regent         |
| 37    | 18716768    | High Lords of Terra     |
| 38    | 20000000    | Emperor of Mankind      |

⚔️ Players advance through these ranks based on their **total XP** earned in matches, with early ranks scaling quickly and higher ranks requiring exponential XP growth.


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

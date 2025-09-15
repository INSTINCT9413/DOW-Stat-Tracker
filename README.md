

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

| Level | XP Required | Rank |
|-------|-------------|------|
| 0  | 0        | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Recruit.png" alt="Recruit" width="25" height="25"> Recruit |
| 1  | 500      | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Conscript.png" alt="Conscript" width="25" height="25"> Conscript |
| 2  | 750      | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Guardsman.png" alt="Guardsman" width="25" height="25"> Guardsman |
| 3  | 1125     | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Corporal.png" alt="Corporal" width="25" height="25"> Corporal |
| 4  | 1687     | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Sergeant.png" alt="Sergeant" width="25" height="25"> Sergeant |
| 5  | 2531     | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/BattleBrother.png" alt="Battle Brother" width="25" height="25"> Battle Brother |
| 6  | 3796     | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Lieutenant.png" alt="Lieutenant" width="25" height="25"> Lieutenant |
| 7  | 5694     | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/TankCommander.png" alt="Tank Commander" width="25" height="25"> Tank Commander |
| 8  | 8541     | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Captain.png" alt="Captain" width="25" height="25"> Captain |
| 9  | 12812    | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Major.png" alt="Major" width="25" height="25"> Major |
| 10 | 19218    | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Colonel.png" alt="Colonel" width="25" height="25"> Colonel |
| 11 | 28827    | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Commissar.png" alt="Commissar" width="25" height="25"> Commissar |
| 12 | 43241    | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/ColonelCommissar.png" alt="Colonel Commissar" width="25" height="25"> Colonel Commissar |
| 13 | 64861    | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Castellan.png" alt="Castellan" width="25" height="25"> Castellan |
| 14 | 97291    | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Codicier.png" alt="Codicier" width="25" height="25"> Codicier |
| 15 | 145936   | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Librarian.png" alt="Librarian" width="25" height="25"> Librarian |
| 16 | 218904   | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/ChiefLibrarian.png" alt="Chief Librarian" width="25" height="25"> Chief Librarian |
| 17 | 328356   | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/ChapterMaster.png" alt="Chapter Master" width="25" height="25"> Chapter Master |
| 18 | 492534   | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/MajorGeneral.png" alt="Major General" width="25" height="25"> Major General |
| 19 | 738801   | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/LieutenantGeneral.png" alt="Lieutenant General" width="25" height="25"> Lieutenant General |
| 20 | 1108201  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Marshal.png" alt="Marshal" width="25" height="25"> Marshal |
| 21 | 1328679  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/General.png" alt="General" width="25" height="25"> General |
| 22 | 1592863  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Inquisitor.png" alt="Inquisitor" width="25" height="25"> Inquisitor |
| 23 | 1909111  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/InquisitorLord.png" alt="Inquisitor Lord" width="25" height="25"> Inquisitor Lord |
| 24 | 2288362  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Master.png" alt="Master" width="25" height="25"> Master |
| 25 | 2740925  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Grandmaster.png" alt="Grandmaster" width="25" height="25"> Grandmaster |
| 26 | 3281084  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/LordGeneral.png" alt="Lord General" width="25" height="25"> Lord General |
| 27 | 3926263  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/LordGeneralMilitant.png" alt="Lord General Militant" width="25" height="25"> Lord General Militant |
| 28 | 4704882  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Warmaster.png" alt="Warmaster" width="25" height="25"> Warmaster |
| 29 | 5633985  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/LordCommander.png" alt="Lord Commander" width="25" height="25"> Lord Commander |
| 30 | 6749468  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/LordCommanderMilitant.png" alt="Lord Commander Militant" width="25" height="25"> Lord Commander Militant |
| 31 | 8087713  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Commandant.png" alt="Commandant" width="25" height="25"> Commandant |
| 32 | 9686716  | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/LordConstable.png" alt="Lord Constable" width="25" height="25"> Lord Constable |
| 33 | 10601427 | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Captain-General.png" alt="Captain-General" width="25" height="25"> Captain-General |
| 34 | 12899419 | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/ChiefCommandant.png" alt="Chief Commandant" width="25" height="25"> Chief Commandant |
| 35 | 14644580 | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/Primarch.png" alt="Primarch" width="25" height="25"> Primarch |
| 36 | 16644580 | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/ImperialRegent.png" alt="Imperial Regent" width="25" height="25"> Imperial Regent |
| 37 | 18716768 | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/HighLordsofTerra.png" alt="High Lords of Terra" width="25" height="25"> High Lords of Terra |
| 38 | 20000000 | <img src="https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/DOW%20Stat%20Tracker/Resources/EmperorofMankind.png" alt="Emperor of Mankind" width="25" height="25"> Emperor of Mankind |

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

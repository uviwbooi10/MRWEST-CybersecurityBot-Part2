#  MrWest Cybersecurity Bot — Part 1, 2 & 3 (Full POE)

**PROG6221 – Portfolio of Evidence – Final Submission**
**Student:** Uviwe Booi
**Student Number:** ST10491015
**Module:** Programming 2A

---

##  Overview

The MrWest Cybersecurity Bot is a WPF GUI application that raises cybersecurity awareness through an interactive chatbot, a task assistant with JSON storage, a cybersecurity quiz mini-game, and an activity log. This repository contains the complete, combined submission for Parts 1, 2, and 3 of the POE.

---

##  Features

### Part 1 — Console Foundations (carried into GUI)
-  Voice greeting on startup (`SoundPlayer`)
-  ASCII art logo (MrWest themed)
-  Personalised name-based interaction

### Part 2 — GUI, Keywords, Sentiment & Memory
-  **Keyword Recognition** – 13+ cybersecurity topics with randomised responses
-  **Random Responses** – multiple responses per topic
-  **Conversation Flow** – "tell me more" continues the current topic
-  **Sentiment Detection** – worried, curious, frustrated, happy
-  **Memory & Recall** – remembers your name and favourite topic
-  Dark cybersecurity-themed WPF interface

### Part 3 — Task Assistant, Quiz, NLP & Activity Log
-  **Task Assistant** – add, view, complete, and delete tasks with reminders, stored in `tasks.json`
-  **Cybersecurity Quiz** – 14 questions across phishing, passwords, safe browsing, social engineering, 2FA, malware, privacy, and data backup, with immediate feedback and a final score
-  **NLP Simulation** – detects task, reminder, quiz, and log intents from varied natural phrasing (e.g. "add task", "I need to", "remind me to")
-  **Activity Log** – timestamped record of every significant action, shows last 10 entries with a "show more" option

---

##  Project Structure

```
MRWEST-CybersecurityBot/
├── App.xaml / App.xaml.cs
├── MainWindow.xaml              → GUI layout (Chat, Tasks, Quiz tabs)
├── MainWindow.xaml.cs           → UI event handlers
├── ChatBot.cs                   → Central routing logic (NLP + Part 1/2 flow)
├── KeywordResponder.cs          → Keyword dictionary, responses, synonym matching
├── SentimentDetector.cs         → Sentiment detection logic
├── MemoryStore.cs               → User memory and recall
├── CyberTask.cs                 → Task model
├── TaskStorageHelper.cs         → Reads/writes tasks.json (Newtonsoft.Json)
├── TaskManager.cs               → Task business logic
├── QuizQuestion.cs              → Quiz question model
├── QuizManager.cs               → Quiz logic, scoring, feedback
├── ActivityLogger.cs            → Logs all significant actions
├── greeting.wav                 → Voice greeting audio
├── tasks.json                   → Auto-created task storage (example included)
├── CyberSecurityChatBot.csproj
├── README.md
└── .github/
    └── workflows/
        └── dotnet.yml           → GitHub Actions CI
```

---

##  How to Run

### Prerequisites
- Visual Studio 2022
- .NET 8.0
- Windows OS
- Newtonsoft.Json NuGet package

### Setup Steps
1. Clone the repository:
```
git clone https://github.com/uviwbooi10/MRWEST-CybersecurityBot-Part2.git
```
2. Open the `.sln` file in Visual Studio 2022
3. Install the Newtonsoft.Json NuGet package:
   - Right-click the project in Solution Explorer
   - Click **Manage NuGet Packages**
   - Click the **Browse** tab
   - Search `Newtonsoft.Json`
   - Click **Install**
4. Place `greeting.wav` in the project root if not already present
5. Set `greeting.wav` → Properties → **Copy to Output Directory** → **Copy Always**
6. Press **F5** to build and run

> **Note:** `tasks.json` is created automatically the first time you add a task — no manual setup required. An example file with one task is included in this repo for reference.

---

##  How to Use

### Chat Tab
- Type your name when prompted
- Ask about cybersecurity topics: `passwords`, `phishing`, `malware`, `privacy`, `scam`, `vpn`, `firewall`, `2fa`, `encryption`, `ransomware`, `browsing`
- Try: `I am worried about phishing` → sentiment detection + auto tip
- Try: `tell me more` → continues the last topic
- Try: `Add a task to enable two-factor authentication` → adds a task via NLP
- Try: `Remind me to update my password tomorrow` → sets a reminder
- Try: `start quiz` → launches the quiz from chat
- Try: `show activity log` → view recent actions

### Tasks Tab
- Fill in title, description, and reminder → click **Add Task**
- Select a task → **Mark Complete** or **Delete**
- Tasks persist in `tasks.json` and reload automatically on restart

### Quiz Tab
- Click **Start Quiz**
- Select an answer with the radio buttons → **Submit Answer**
- Immediate feedback shown after each question
- Final score and message displayed after question 14

---

##  CI/CD

GitHub Actions automatically builds the project on every push to `main` using `windows-latest` for WPF compatibility.

![CI Status](https://github.com/uviwbooi10/MRWEST-CybersecurityBot-Part2/actions/workflows/dotnet.yml/badge.svg)

---

##  Releases

| Tag | Description |
|-----|-------------|
| v2.0 | Part 2 initial release — WPF GUI, keyword recognition, voice greeting, ASCII art |
| v2.1 | Part 2 full feature release — sentiment detection, memory, conversation flow |
| v3.0 | Part 3 — Task Assistant with JSON storage |
| v3.1 | Part 3 — Quiz mini-game and Activity Log |
| v3.2 | Part 3 — Final release, full integration of Parts 1, 2 and 3 |

---

##  Video Presentation

 YouTube Link: **[https://youtu.be/gxQ-4JRDRyc?si=xFNaWoutMhguY44F]**

---

##  Screenshots

### App Running
![App Screenshot] <img width="1918" height="1078" alt="Screenshot 2026-06-26 032418" src="https://github.com/user-attachments/assets/9983be40-3580-45f1-862c-55dc2e739d5d" />


### GitHub Actions Green Tick
![CI Screenshot]<img width="1918" height="975" alt="Screenshot 2026-06-26 033853" src="https://github.com/user-attachments/assets/62f51e2b-69b6-4fbd-af70-61284ef917c1" />

(<img width="1912" height="1022" alt="Screenshot 2026-06-26 033938" src="https://github.com/user-attachments/assets/e14d572b-e02f-47f6-bfb0-d8f466d74902" />



---

##  References

- Microsoft Docs – [WPF Overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- Microsoft Docs – [SoundPlayer](https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer)
- Newtonsoft.Json – [https://www.newtonsoft.com/json](https://www.newtonsoft.com/json)
- SABRIC – [South African Banking Risk Information Centre](https://www.sabric.co.za)
- POPIA – [Protection of Personal Information Act](https://popia.co.za)

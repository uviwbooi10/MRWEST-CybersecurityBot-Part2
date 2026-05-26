# 🔐 MrWest Cybersecurity Bot — Part 2

**PROG6221 – Portfolio of Evidence – Part 2**  
**Student:** Uviwe Booi  
**Student Number:** ST10491015  
**Module:** Programming 2A  

---

## 📋 Overview

The MrWest Cybersecurity Bot is a WPF GUI application that raises cybersecurity awareness. The bot greets users with a voice message and ASCII art logo, asks for their name, and answers cybersecurity questions in an interactive chat interface. Part 2 adds keyword recognition, random responses, sentiment detection, memory and recall, and conversation flow.

---

## ✅ Features

- 🎤 **Voice Greeting** – Plays `greeting.wav` on startup
- 🎨 **ASCII Art Logo** – MrWest themed banner in the GUI header
- 👤 **User Memory** – Remembers your name and favourite topic
- 🤖 **Keyword Recognition** – 13 cybersecurity topics with targeted responses
- 🎲 **Random Responses** – Multiple responses per topic, randomly selected
- 💬 **Conversation Flow** – Type 'tell me more' to continue any topic
- 🧠 **Sentiment Detection** – Detects worried, curious, frustrated, happy
- 🛡️ **Error Handling** – Handles empty and unknown input gracefully
- 💻 **Dark Cybersecurity GUI** – Clean WPF interface with green accent colours

---

## 💬 Supported Keywords

| Keyword | Example input |
|---------|--------------|
| password | tell me about passwords |
| phishing | what is phishing |
| malware | how does malware work |
| privacy | how do I protect my privacy |
| scam | what is a scam |
| vpn | what is a vpn |
| firewall | tell me about firewalls |
| two-factor | what is 2fa |
| social engineering | what is social engineering |
| encryption | tell me about encryption |
| data breach | what is a data breach |
| ransomware | tell me about ransomware |
| browsing | safe browsing tips |

---

## 🚀 How to Run

### Prerequisites
- Visual Studio 2022
- .NET 8.0
- Windows OS

### Steps
1. Clone the repository:
2. 2. Open `CyberSecurityChatBot.csproj` in Visual Studio 2022
3. Place `greeting.wav` in the project root
4. Set `greeting.wav` → Properties → Copy to Output Directory → **Copy Always**
5. Press **F5** or click the Run button to launch

---

## 🎤 Voice Greeting

Place your `greeting.wav` file in the project root folder. The app will automatically play it on startup. The file is configured to copy to the output directory on every build.

---

## 🔁 CI/CD

GitHub Actions automatically builds the project on every push to `main` using `windows-latest` for WPF compatibility.

![CI Status](https://github.com/uviwbooi10/MRWEST-CybersecurityBot-Part2/actions/workflows/dotnet.yml/badge.svg)

---

## 📹 Video Presentation

🎬 YouTube Link: **[ADD YOUR YOUTUBE LINK HERE]**

---

## 📸 Screenshots

### App Running
https://github.com/uviwbooi10/MRWEST-CybersecurityBot-Part2/blob/main/Screenshot%202026-05-26%20214948.png?raw=true

### GitHub Actions Green Tick
<img width="1915" height="1029" alt="Screenshot 2026-05-26 215553" src="https://github.com/user-attachments/assets/3873cda0-6d95-49d7-b5d1-96984cc83e17" />


---

## 📚 References

- Microsoft Docs – [WPF Overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- Microsoft Docs – [SoundPlayer](https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer)
- SABRIC – [South African Banking Risk Information Centre](https://www.sabric.co.za)
- POPIA – [Protection of Personal Information Act](https://popia.co.za)

## 🗂️ Project Structure

using PART2;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PART2
{
    /// <summary>
    /// Code-behind for the main WPF window.
    /// This file only handles UI events — all logic lives in ChatBot.cs.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ChatBot _chatBot;

        public MainWindow()
        {
            InitializeComponent();
            _chatBot = new ChatBot();

            PlayVoiceGreeting();
            AppendBotMessage(_chatBot.GetGreeting());
            UserInputBox.Focus();
        }

        // ─────────────────────────────────────────────────────
        // UI Event Handlers
        // ─────────────────────────────────────────────────────

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage();
        }

        // ─────────────────────────────────────────────────────
        // Core Send Logic
        // ─────────────────────────────────────────────────────

        private void SendMessage()
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUserMessage(input);
            UserInputBox.Clear();

            string response = _chatBot.ProcessInput(input);
            AppendBotMessage(response);

            UpdateStatus($"Last message: {DateTime.Now:HH:mm:ss}");
        }

        // ─────────────────────────────────────────────────────
        // Chat Display Helpers
        // ─────────────────────────────────────────────────────

        private void AppendUserMessage(string message)
        {
            var container = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(26, 26, 46)),
                CornerRadius = new CornerRadius(12, 12, 2, 12),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(80, 4, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var label = new TextBlock
            {
                Text = $" You",
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 180)),
                Margin = new Thickness(0, 0, 0, 4)
            };

            var text = new TextBlock
            {
                Text = message,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 255)),
                TextWrapping = TextWrapping.Wrap
            };

            var stack = new StackPanel();
            stack.Children.Add(label);
            stack.Children.Add(text);
            container.Child = stack;

            ChatPanel.Children.Add(container);
            ScrollToBottom();
        }

        private void AppendBotMessage(string message)
        {
            var container = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(10, 20, 30)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 156)),
                BorderThickness = new Thickness(2, 0, 0, 0),
                CornerRadius = new CornerRadius(2, 12, 12, 12),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(0, 4, 80, 4),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var label = new TextBlock
            {
                Text = " MrWest Bot",
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 156)),
                Margin = new Thickness(0, 0, 0, 4)
            };

            var text = new TextBlock
            {
                Text = message,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(200, 220, 210)),
                TextWrapping = TextWrapping.Wrap
            };

            var stack = new StackPanel();
            stack.Children.Add(label);
            stack.Children.Add(text);
            container.Child = stack;

            ChatPanel.Children.Add(container);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.UpdateLayout();
            ChatScrollViewer.ScrollToBottom();
        }

        private void UpdateStatus(string message)
        {
            StatusBar.Text = $" MrWest Cybersecurity Bot — {message}";
        }

        // ─────────────────────────────────────────────────────
        // Voice Greeting
        // ─────────────────────────────────────────────────────

        private void PlayVoiceGreeting()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            string wavPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            if (!System.IO.File.Exists(wavPath)) return;

            try
            {
                var player = new SoundPlayer(wavPath);
                player.Play();
            }
            catch { /* Silently skip if audio fails */ }
        }
    }
}
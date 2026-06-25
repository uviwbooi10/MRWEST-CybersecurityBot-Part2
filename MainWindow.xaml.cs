using THEPART2;
using System;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace THEPART2
{
    public partial class MainWindow : Window
    {
        private readonly ChatBot _chatBot;
        private string? _selectedQuizAnswer;
        private int _selectedTaskId = -1;

        public MainWindow()
        {
            InitializeComponent();
            _chatBot = new ChatBot();

            PlayVoiceGreeting();
            AppendBotMessage(_chatBot.GetGreeting());
            UserInputBox.Focus();

            RefreshTaskList();
        }

        // ═══════════════════════════════════════════════════
        // CHAT TAB
        // ═══════════════════════════════════════════════════

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }

        private void SendMessage()
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendUserMessage(input);
            UserInputBox.Clear();

            string response = _chatBot.ProcessInput(input);
            AppendBotMessage(response);

            // Keep the Tasks tab in sync if a task was added via chat NLP
            RefreshTaskList();
        }

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
                Text = " You",
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

        // ═══════════════════════════════════════════════════
        // TASKS TAB
        // ═══════════════════════════════════════════════════

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            string description = TaskDescriptionBox.Text.Trim();
            string reminder = TaskReminderBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing Title",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
                description = $"Complete the task: {title}.";

            _chatBot.Tasks.AddTask(title, description, reminder);

            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            TaskReminderBox.Clear();

            RefreshTaskList();
        }

        private void MarkCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTaskId == -1)
            {
                MessageBox.Show("Please select a task first.", "No Task Selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _chatBot.Tasks.MarkAsComplete(_selectedTaskId);
            RefreshTaskList();
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTaskId == -1)
            {
                MessageBox.Show("Please select a task first.", "No Task Selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _chatBot.Tasks.DeleteTask(_selectedTaskId);
            _selectedTaskId = -1;
            RefreshTaskList();
        }

        private void RefreshTasksButton_Click(object sender, RoutedEventArgs e) => RefreshTaskList();

        private void RefreshTaskList()
        {
            var tasks = _chatBot.Tasks.GetAllTasks();
            TaskListBox.Items.Clear();

            foreach (var task in tasks.OrderBy(t => t.Id))
            {
                string status = task.IsComplete ? "✅" : "⬜";
                string reminderText = string.IsNullOrEmpty(task.Reminder) ? "" : $" |  {task.Reminder}";

                var item = new ListBoxItem
                {
                    Content = $"{status} [#{task.Id}] {task.Title} — {task.Description}{reminderText}",
                    Tag = task.Id,
                    Foreground = task.IsComplete
                        ? new SolidColorBrush(Color.FromRgb(100, 180, 130))
                        : new SolidColorBrush(Color.FromRgb(224, 224, 224))
                };
                item.Selected += (s, e) => _selectedTaskId = (int)((ListBoxItem)s!).Tag;

                TaskListBox.Items.Add(item);
            }
        }

        // ═══════════════════════════════════════════════════
        // QUIZ TAB
        // ═══════════════════════════════════════════════════

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            string firstQuestionText = _chatBot.StartQuizFromGui();
            QuizFeedbackText.Text = "";
            RenderCurrentQuizQuestion();
        }

        private void RenderCurrentQuizQuestion()
        {
            var question = _chatBot.GetCurrentQuizQuestion();
            QuizOptionsPanel.Children.Clear();
            _selectedQuizAnswer = null;

            if (question == null)
            {
                QuizQuestionText.Text = "Quiz complete! Check the feedback above for your final score.";
                QuizProgressText.Text = "Quiz finished";
                SubmitAnswerButton.IsEnabled = false;
                return;
            }

            QuizProgressText.Text = $"Question in progress...";
            QuizQuestionText.Text = $"[{question.Topic}]\n\n{question.Question}";

            foreach (var option in question.Options)
            {
                string optionValue = question.IsTrueFalse
                    ? option
                    : option.Substring(0, 1); // "A) text" -> "A"

                var radio = new RadioButton
                {
                    Content = option,
                    GroupName = "QuizOptions",
                    Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 13,
                    Margin = new Thickness(0, 6, 0, 6),
                    Tag = optionValue
                };
                radio.Checked += (s, e) =>
                {
                    _selectedQuizAnswer = (string)((RadioButton)s!).Tag;
                    SubmitAnswerButton.IsEnabled = true;
                };

                QuizOptionsPanel.Children.Add(radio);
            }
        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedQuizAnswer))
            {
                MessageBox.Show("Please select an answer first.", "No Answer Selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string result = _chatBot.SubmitQuizAnswerFromGui(_selectedQuizAnswer);

            // result contains feedback + next question text combined; split for display
            QuizFeedbackText.Text = result;
            SubmitAnswerButton.IsEnabled = false;

            RenderCurrentQuizQuestion();
        }

        // ═══════════════════════════════════════════════════
        // VOICE GREETING
        // ═══════════════════════════════════════════════════

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

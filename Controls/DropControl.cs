using Microsoft.Win32;
using MvvmCross.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CvProject.Wpf.Controls
{
    public class DropControl : Control
    {
        private readonly Geometry Block = Geometry.Parse("M7 24Q7 27.05 8.1 29.875Q9.2 32.7 11.05 34.95L34.95 11.05Q32.65 9.1 29.85 8.05Q27.05 7 24 7Q16.9 7 11.95 11.95Q7 16.9 7 24ZM13.05 37Q15.3 38.95 18.125 39.975Q20.95 41 24 41Q31.1 41 36.05 36.05Q41 31.1 41 24Q41 20.95 39.95 18.15Q38.9 15.35 37 13.05ZM4 24Q4 19.85 5.575 16.2Q7.15 12.55 9.85 9.85Q12.55 7.15 16.2 5.575Q19.85 4 24 4Q28.15 4 31.8 5.575Q35.45 7.15 38.15 9.85Q40.85 12.55 42.425 16.2Q44 19.85 44 24Q44 28.15 42.425 31.8Q40.85 35.45 38.15 38.15Q35.45 40.85 31.8 42.425Q28.15 44 24 44Q19.85 44 16.2 42.425Q12.55 40.85 9.85 38.15Q7.15 35.45 5.575 31.8Q4 28.15 4 24Z");
        private readonly Geometry PlaceItem = Geometry.Parse("M9 42Q7.8 42 6.9 41.1Q6 40.2 6 39V17Q6 15.8 6.9 14.9Q7.8 14 9 14H19.5V17H9Q9 17 9 17Q9 17 9 17V39Q9 39 9 39Q9 39 9 39H39Q39 39 39 39Q39 39 39 39V17Q39 17 39 17Q39 17 39 17H28.5V14H39Q40.2 14 41.1 14.9Q42 15.8 42 17V39Q42 40.2 41.1 41.1Q40.2 42 39 42ZM24 31.85 15.9 23.75 18.05 21.6 22.5 26.05V0H25.5V26.05L29.95 21.6L32.1 23.75Z");
        private readonly Geometry Accept = Geometry.Parse("M23.05 33.6 11.95 22.5 14.05 20.4 23.05 29.4 42.15 10.3 44.25 12.4ZM9 42Q7.75 42 6.875 41.125Q6 40.25 6 39V9Q6 7.75 6.875 6.875Q7.75 6 9 6H39Q39.7 6 40.275 6.3Q40.85 6.6 41.2 7L39 9.2Q39 9.2 39 9.1Q39 9 39 9H9Q9 9 9 9Q9 9 9 9V39Q9 39 9 39Q9 39 9 39H39Q39 39 39 39Q39 39 39 39V21.85L42 18.85V39Q42 40.25 41.125 41.125Q40.25 42 39 42Z");
        private readonly SolidColorBrush Blue = new SolidColorBrush(Color.FromRgb(67, 84, 248));
        private readonly SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(231, 76, 60));
        private readonly SolidColorBrush Green = new SolidColorBrush(Color.FromRgb(46, 204, 113));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DropControl), new PropertyMetadata("Drag your image here to start processing."));

        public IMvxAsyncCommand<string> FileSelectedCommand
        {
            get { return (IMvxAsyncCommand<string>)GetValue(FileSelectedCommandProperty); }
            set { SetValue(FileSelectedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileSelectedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileSelectedCommandProperty =
            DependencyProperty.Register("FileSelectedCommand", typeof(IMvxAsyncCommand<string>), typeof(DropControl), new PropertyMetadata(null));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(DropControl), new PropertyMetadata(null));

        private Button BrowseButton_PART;
        private Border Border_PART;
        private Path Path_PART;
        private bool CanDrop = true;
        public override void OnApplyTemplate()
        {
            Path_PART = GetTemplateChild(nameof(Path_PART)) as Path;
            Path_PART.Data = PlaceItem;
            Path_PART.Fill = Blue;
            Border_PART = GetTemplateChild(nameof(Border_PART)) as Border;
            Border_PART.DragEnter += Border_PART_DragEnter;
            Border_PART.DragLeave += Border_PART_DragLeave;
            Border_PART.Drop += Border_PART_Drop;

            BrowseButton_PART = GetTemplateChild(nameof(BrowseButton_PART)) as Button;
            BrowseButton_PART.Click += BrowseButton_Click;
        }

        private void Border_PART_DragLeave(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            Path_PART.Data = PlaceItem;
            Path_PART.Fill = Blue;
            CanDrop = true;
        }

        private void Border_PART_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    var ext = System.IO.Path.GetExtension(file).ToUpper();
                    if (!Filter.Contains(ext) || files.Length > 1)
                    {
                        Path_PART.Data = Block;
                        Path_PART.Fill = Red;
                        CanDrop = false;
                    }
                    else
                    {
                        Path_PART.Data = Accept;
                        Path_PART.Fill = Green;
                        CanDrop = true;
                    }
                }
            }
        }

        private void Border_PART_Drop(object sender, DragEventArgs e)
        {
            if (CanDrop)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                FileSelectedCommand?.Execute(files[0]);
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = Filter };
            if(dialog.ShowDialog() == true)
                FileSelectedCommand?.Execute(dialog.FileName);
            else
                FileSelectedCommand?.Execute(null);
        }

        static DropControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropControl), new FrameworkPropertyMetadata(typeof(DropControl)));
        }
    }
}

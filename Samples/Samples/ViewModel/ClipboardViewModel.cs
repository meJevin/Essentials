using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Samples.ViewModel
{
    class ClipboardViewModel : BaseViewModel
    {
        string fieldValue;
        string lastCopied;

        public ClipboardViewModel()
        {
            CopyCommand = new Command(OnCopy);
            CopyImageCommand = new Command(OnCopyImage);
            PasteCommand = new Command(OnPaste);
            CheckCommand = new Command(OnCheck);
        }

        public ICommand CopyCommand { get; }

        public ICommand CopyImageCommand { get; }

        public ICommand PasteCommand { get; }

        public ICommand CheckCommand { get; }

        public string FieldValue
        {
            get => fieldValue;
            set => SetProperty(ref fieldValue, value);
        }

        public string LastCopied
        {
            get => lastCopied;
            set => SetProperty(ref lastCopied, value);
        }

        public override void OnAppearing()
        {
            try
            {
                Clipboard.ClipboardContentChanged += OnClipboardContentChanged;
            }
            catch (FeatureNotSupportedException)
            {
            }
        }

        public override void OnDisappearing()
        {
            try
            {
                Clipboard.ClipboardContentChanged -= OnClipboardContentChanged;
            }
            catch (FeatureNotSupportedException)
            {
            }
        }

        void OnClipboardContentChanged(object sender, EventArgs args)
        {
            LastCopied = $"Last copied text at {DateTime.UtcNow:T}";
        }

        async void OnCopy()
        {
            await Clipboard.SetTextAsync(FieldValue);
        }

        async void OnCopyImage()
        {
            await Clipboard.SetImageAsync("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");
        }

        async void OnPaste()
        {
            var text = await Clipboard.GetTextAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                FieldValue = text;
            }
        }

        async void OnCheck()
        {
            await DisplayAlertAsync($"Has text: {Clipboard.HasText}");
        }
    }
}

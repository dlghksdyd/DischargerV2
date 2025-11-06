using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace DischargerV2.Modal
{
    public static class ModalManager
    {
        public static ModalBaseView View { get; private set; }
        private static ModalBaseViewModel ViewModel { get; set; }
        
        private static readonly Dictionary<object, TaskCompletionSource<ModalResult>> _pending = new Dictionary<object, TaskCompletionSource<ModalResult>>();

        public static void Configure(Brush background)
        {
            if (background == null) throw new ArgumentNullException(nameof(background));

            View = new ModalBaseView();
            ViewModel = new ModalBaseViewModel(background);
            View.DataContext = ViewModel;
        }

        public static void Configure(string backgroundHex)
        {
            if (string.IsNullOrWhiteSpace(backgroundHex))
                throw new ArgumentNullException(nameof(backgroundHex));

            try
            {
                var color = (Color)ColorConverter.ConvertFromString(backgroundHex);
                var brush = new SolidColorBrush(color);
                Configure(brush);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"배경 색상 문자열이 올바르지 않습니다. 입력값: '{backgroundHex}'", nameof(backgroundHex));
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"배경 색상 변환 중 오류 발생: {backgroundHex}", nameof(backgroundHex), ex);
            }
        }

        public static void RegisterView<TView, TViewModel>()
        {
            if (View == null || ViewModel == null)
                throw new InvalidOperationException("ModalManager.Configure() must be called before registering views.");
            
            var template = new DataTemplate(typeof(TViewModel));

            var factory = new FrameworkElementFactory(typeof(TView));
            factory.SetBinding(FrameworkElement.DataContextProperty, new Binding());

            template.VisualTree = factory;

            var key = new DataTemplateKey(typeof(TViewModel));
            View.Resources[key] = template;
        }

        /// <summary>
        /// 동기 모달 Open. UI 스레드에서 호출해야 합니다.
        /// </summary>
        public static ModalResult Open(object viewmodel)
        {
            if (ViewModel == null)
                throw new InvalidOperationException("ModalManager.Configure() must be called before opening popups.");

            var dispatcher = Application.Current?.Dispatcher ?? throw new InvalidOperationException("No current Application/Dispatcher found.");
            if (!dispatcher.CheckAccess())
                throw new InvalidOperationException("ModalManager.Open must be called on the UI thread.");

            View?.Focus();

            var tcs = new TaskCompletionSource<ModalResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pending[viewmodel] = tcs;
            
            ViewModel.OpenPopup(viewmodel);

            var frame = new DispatcherFrame();
            tcs.Task.ContinueWith(_ => frame.Continue = false, TaskScheduler.FromCurrentSynchronizationContext());

            Dispatcher.PushFrame(frame);

            return tcs.Task.Result;
        }

        /// <summary>
        /// 팝업 닫기(+ 모달 결과 전달).
        /// </summary>
        public static void Close(object viewmodel, ModalResult result = ModalResult.None)
        {
            if (ViewModel == null) return;

            TaskCompletionSource<ModalResult> tcs;
            if (_pending.TryGetValue(viewmodel, out tcs))
            {
                _pending.Remove(viewmodel);
                if (tcs != null)
                {
                    tcs.TrySetResult(result);
                }
            }

            ViewModel.ClosePopup(viewmodel);
        }
    }
}

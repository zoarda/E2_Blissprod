
namespace Naninovel.UI
{
    public class ExternalScriptsBrowserReturnButton : ScriptableButton
    {
        private ExternalScriptsBrowserPanel externalScriptsBrowser;

        protected override void Awake ()
        {
            base.Awake();

            externalScriptsBrowser = GetComponentInParent<ExternalScriptsBrowserPanel>();
        }

        protected override void OnButtonClick () => externalScriptsBrowser.Hide();
    }
}

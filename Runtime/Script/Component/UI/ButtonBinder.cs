
using UnityEngine;
using UnityEngine.UI;

namespace Aya.DataBinding
{
    [AddComponentMenu("Data Binding/Button Binder")]
    public class ButtonBinder : ComponentBinder<Button, ICommand, RuntimeButtonBinder>
    {
        public override bool NeedUpdate => true;
    }

    public class RuntimeButtonBinder : DataBinder<Button, ICommand>
    {
        public override bool NeedUpdate => true;

        public override ICommand Value
        {
            get => _command;
            set
            {
                if(_command != null)
                {
                    Target.onClick.RemoveListener(_command.Execute);
                }

                if(value != null)
                {
                    Target.interactable = value.CanExecute();
                    Target.onClick.AddListener(value.Execute);
                }

                _command = value;
            }
        }

        public override void UpdateSource()
        {
            base.UpdateSource();

            if(_command != null)
            {
                Target.interactable = _command.CanExecute();
            }
        }

        private ICommand _command;
    }

    public interface ICommand
    {
        bool CanExecute();
        void Execute();
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(ButtonBinder)), UnityEditor.CanEditMultipleObjects]
    public class ButtonBinderEditor : ComponentBinderEditor<Button, ICommand, RuntimeButtonBinder>
    {
    }

#endif
}

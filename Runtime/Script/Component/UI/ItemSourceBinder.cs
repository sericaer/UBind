
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Aya.DataBinding
{
    [AddComponentMenu("Data Binding/ItemSource Binder")]
    public class ItemSourceBinder : ComponentBinder<LayoutGroup, IEnumerable<object>, RuntimeItemSourceBinder>
    {
        public override bool NeedUpdate => true;

        public GameObject ItemTemplate;

        public override RuntimeItemSourceBinder CreateDataBinder()
        {
            var dataBinder = new RuntimeItemSourceBinder
            {
                Target = Target,
                Context = Context,
                Direction = Direction,
                Key = Key,
                ItemTemplate = ItemTemplate
            };

            return dataBinder;
        }
    }

    public class RuntimeItemSourceBinder : DataBinder<LayoutGroup, IEnumerable<object>>
    {
        public override bool NeedUpdate => true;

        public GameObject ItemTemplate { get; set; }

        private Dictionary<object, GameObject> itemData2Obj = new Dictionary<object, GameObject>();

        public override IEnumerable<object> Value
        {
            get => _items;
            set
            {
                foreach(var gmObj in itemData2Obj.Values)
                {
                    Object.Destroy(gmObj);
                }

                foreach (var key in itemData2Obj.Keys)
                {
                    itemData2Obj.Remove(key);
                }

                _items = value;
            }
        }



        public override void UpdateSource()
        {
            base.UpdateSource();

            if(_items == null)
            {
                return;
            }

            foreach (var item in _items)
            {
                if(!itemData2Obj.ContainsKey(item))
                {
                    itemData2Obj.Add(item, Object.Instantiate(ItemTemplate, ItemTemplate.transform.parent));
                }
            }

            foreach(var key in itemData2Obj.Keys)
            {
                if(!_items.Contains(key))
                {
                    Object.Destroy(itemData2Obj[key]);
                    itemData2Obj.Remove(key);
                }
            }    
        }

        private IEnumerable<object> _items;
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(ItemSourceBinder)), UnityEditor.CanEditMultipleObjects]
    public class ItemSourceBinderEditor : ComponentBinderEditor<LayoutGroup, IEnumerable<object>, RuntimeItemSourceBinder>
    {
        public ItemSourceBinder Binder => target as ItemSourceBinder;
        protected SerializedProperty ItemTemplate;

        public override void OnEnable()
        {
            base.OnEnable();
            ItemTemplate = serializedObject.FindProperty(nameof(Binder.ItemTemplate));
        }

        public override void DrawBody()
        {
            EditorGUILayout.PropertyField(ItemTemplate);
        }
    }

#endif
}

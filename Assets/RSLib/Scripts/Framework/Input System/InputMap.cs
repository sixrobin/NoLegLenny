namespace RSLib.Framework.InputSystem
{
    using System.Linq;
    using System.Xml.Linq;
    using UnityEngine;

    /// <summary>
    /// Contains an input map informations.
    /// </summary>
    public class InputMap
    {
        /// <summary>
        /// Key is the action id (representing a button and NOT an axis).
        /// Value is a KeyBinding class containing inputs and other action related datas.
        /// </summary>
        private System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> _map = new System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding>();
        
        public System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> MapCopy => new System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding>(_map);

        public InputMap()
        {
        }

        public InputMap(InputMapDatas mapDatas)
        {
            GenerateMap(mapDatas);
        }

        public InputMap(XContainer container)
        {
            Deserialize(container);
        }

        public InputMap(InputMap inputMap)
        {
            _map = inputMap._map;
        }

        public InputMap(System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> map)
        {
            _map = map;
        }

        /// <summary>
        /// Clears the map dictionary.
        /// </summary>
        public void Clear()
        {
            _map.Clear();
        }

        /// <summary>
        /// Generates map using ScriptableObject datas as template.
        /// </summary>
        /// <param name="mapDatas">Template datas.</param>
        public void GenerateMap(InputMapDatas mapDatas)
        {
            Clear();

            for (int i = 0; i < mapDatas.Bindings.Length; ++i)
            {
                InputMapDatas.KeyBinding keyBinding = new InputMapDatas.KeyBinding(
                    mapDatas.Bindings[i].ActionId,
                    mapDatas.Bindings[i].KeyCodes,
                    mapDatas.Bindings[i].UserAssignable);

                CreateAction(mapDatas.Bindings[i].ActionId, keyBinding);
            }

            InputManager.Instance.Log($"Generated {_map.Count} input bindings.", InputManager.Instance.gameObject);
        }

        /// <summary>
        /// Deserializes saved datas of an input map.
        /// </summary>
        /// <param name="container">Saved map XContainer.</param>
        /// <returns>True if deserialization has been done successfully, else false.</returns>
        public void Deserialize(XContainer container)
        {
            InputMap defaultMap = InputManager.GetDefaultMapCopy();

            XElement inputMapElement = container.Element(InputManager.INPUT_MAP_ELEMENT_NAME);
            foreach (XElement keyBindingElement in inputMapElement.Elements())
            {
                string actionId = keyBindingElement.Name.LocalName;

                XAttribute btnAttribute = keyBindingElement.Attribute(InputManager.BTN_ATTRIBUTE_NAME);
                if (!System.Enum.TryParse(btnAttribute.Value, out KeyCode btnKeyCode))
                {
                    InputManager.Instance.LogError($"Could not parse {btnAttribute.Value} to a valid UnityEngine.KeyCode. Restoring default input mapping.", InputManager.Instance.gameObject);
                    return;
                }

                XAttribute altBtnAttribute = keyBindingElement.Attribute(InputManager.ALT_ATTRIBUTE_NAME);
                if (!System.Enum.TryParse(altBtnAttribute.Value, out KeyCode altBtnKeyCode))
                {
                    InputManager.Instance.LogError($"Could not parse {altBtnAttribute.Value} to a valid UnityEngine.KeyCode. Restoring default input mapping.", InputManager.Instance.gameObject);
                    return;
                }

                CreateAction(actionId, new InputMapDatas.KeyBinding(actionId, (btnKeyCode, altBtnKeyCode), defaultMap.GetActionBinding(actionId).UserAssignable));
            }
        }

        /// <summary>
        /// Used to sort input map after potentially generating missing inputs, because the user removed them from data files
        /// or because he's loading an older version of the application. Generated inputs are added at the end of the map, so we want
        /// to sort them based on the default map datas.
        /// </summary>
        /// <param name="mapDatas">Map datas to get the actions order from.</param>
        /// <returns>True if sorting has been done successfully, else false.</returns>
        public bool SortActionsBasedOnDatas(InputMapDatas mapDatas)
        {
            try
            {
                System.Collections.Generic.List<string> bindingsList = mapDatas.Bindings.ToList().Select(o => o.ActionId).ToList();
                _map = _map.OrderBy(o => bindingsList.IndexOf(o.Key)).ToDictionary(k => k.Key, v => v.Value);
            }
            catch (System.Exception e)
            {
                InputManager.Instance.LogError($"Error while sorting actions map based on map datas : {e.Message}.", InputManager.Instance.gameObject);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if map contains a given action.
        /// </summary>
        /// <param name="actionId">Action Id to look for.</param>
        /// <returns>True if map contains the action, else false.</returns>
        public bool HasAction(string actionId)
        {
            return _map.ContainsKey(actionId);
        }

        /// <summary>
        /// Creates an action, added to the map dictionary.
        /// </summary>
        /// <param name="actionId">Action Id type to create.</param>
        /// <param name="btns">Both action KeyCodes.</param>
        public void CreateAction(string actionId, InputMapDatas.KeyBinding keyBinding)
        {
            UnityEngine.Assertions.Assert.IsFalse(HasAction(actionId), $"Trying to create an already know action Id {actionId}.");
            _map.Add(actionId, keyBinding);
        }

        /// <summary>
        /// Gets both valid KeyCodes for a given action.
        /// </summary>
        /// <param name="actionId">Action Id to get KeyCodes of.</param>
        /// <returns>Tuple containing KeyCodes.</returns>
        public InputMapDatas.KeyBinding GetActionBinding(string actionId)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(actionId), $"Looking for keyCodes of unknown action Id {actionId}.");
            return _map[actionId];
        }

        /// <summary>
        /// Overrides a KeyCode for a given action and resets other actions that were using the same KeyCode.
        /// </summary>
        /// <param name="actionId">Action Id to override KeyCode of.</param>
        /// <param name="keyCode">KeyCode to set.</param>
        /// <param name="alt">Set the base button or the alternate one.</param>
        public void SetActionButton(string actionId, KeyCode keyCode, bool alt)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(actionId), $"Trying to set keyCode of unknown action Id {actionId}.");

            InputMapDatas.KeyBinding newBinding = _map[actionId];

            if (alt)
                newBinding.SetAltBtn(keyCode);
            else
                newBinding.SetBtn(keyCode);

            string[] keys = _map.Keys.ToArray();
            for (int i = keys.Length - 1; i >= 0; --i)
            {
                if (keys[i] == actionId)
                    continue;

                if (_map[keys[i]].KeyCodes.btn == keyCode)
                    _map[keys[i]].SetBtns(KeyCode.None, _map[keys[i]].KeyCodes.altBtn);

                if (_map[keys[i]].KeyCodes.altBtn == keyCode)
                    _map[keys[i]].SetBtns(_map[keys[i]].KeyCodes.btn, KeyCode.None);
            }

            _map[actionId] = newBinding;
        }
    }
}
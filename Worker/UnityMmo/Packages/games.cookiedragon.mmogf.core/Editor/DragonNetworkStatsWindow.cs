#if UNITY_EDITOR

using Mmogf.Core;
using Mmogf.Core.Networking;
using Mmogf.Unity.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.MemoryProfiler;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace MessagePack.Unity.Editor
{
    internal class DragonNetworkStatsWindow : EditorWindow
    {
        static DragonNetworkStatsWindow window;

        [NonSerialized] bool processInitialized;
        [NonSerialized] CommonHandler _selectedConnection;
        [NonSerialized] DataStat _selectedStat;

        CommonHandler[] _connections;

        [SerializeField] TreeViewState m_TreeViewReceivedState; // Serialized in the window layout file so it survives assembly reloading
        CommandTreeView _treeReceivedView;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderReceivedState;

        [SerializeField] TreeViewState m_TreeViewSentState; // Serialized in the window layout file so it survives assembly reloading
        CommandTreeView _treeSentView;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderSentState;



        [MenuItem("DragonGF/Network Stats", priority = 105)]
        public static void OpenWindow()
        {
            try
            {
                if (window != null)
                {
                    window.Close();
                }
            }
            catch
            {

            }
            

            // will called OnEnable(singleton instance will be set).
            GetWindow<DragonNetworkStatsWindow>("DragonGF Network Statistics").Show();
        }

        Rect _treeViewRect
        {
            get { return new Rect(10, 30, position.width - 20, position.height / 2f - 40); }
        }

        Rect _sentViewRect
        {
            get { return new Rect(10, position.height / 2f + 20, position.width - 20, position.height / 2f - 40); }
        }


        void OnEnable()
        {
            window = this; // set singleton.
            Init();
            EditorApplication.playModeStateChanged += PlayModeState;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayModeState;

        }

        private void PlayModeState(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.EnteredPlayMode)
            {
                processInitialized = false;
            }
        }

        void Init()
        {

            _connections = GameObject.FindObjectsOfType<CommonHandler>();

            if(processInitialized)
                return;

            // Check if it already exists (deserialized from window layout file or scriptable object)
            if (m_TreeViewReceivedState == null)
                m_TreeViewReceivedState = new TreeViewState();

            bool firstInit = m_MultiColumnHeaderReceivedState == null;
            var headerState = CommandTreeView.CreateDefaultMultiColumnHeaderState(_treeViewRect.width);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderReceivedState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderReceivedState, headerState);
            m_MultiColumnHeaderReceivedState = headerState;

            var multiColumnHeader = new MyMultiColumnHeader(headerState);
            if (firstInit)
                multiColumnHeader.ResizeToFit();

            var treeModel = new TreeModel<CommandDataElement>(GetData(true));

            _treeReceivedView = new CommandTreeView(m_TreeViewReceivedState, multiColumnHeader, treeModel);


            //sent
            if (m_TreeViewSentState == null)
                m_TreeViewSentState = new TreeViewState();

            var headerSentState = CommandTreeView.CreateDefaultMultiColumnHeaderState(_treeViewRect.width);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderSentState, headerSentState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderSentState, headerSentState);
            m_MultiColumnHeaderSentState = headerSentState;

            var multiColumnSentHeader = new MyMultiColumnHeader(headerSentState);
            if (firstInit)
                multiColumnSentHeader.ResizeToFit();

            var treeSentModel = new TreeModel<CommandDataElement>(GetData(true));

            _treeSentView = new CommandTreeView(m_TreeViewReceivedState, multiColumnSentHeader, treeSentModel);

            processInitialized = true;
        }

        IList<CommandDataElement> GetData(bool received)
        {
            // generate some test data
            //https://docs.unity3d.com/Manual/TreeViewAPI.html

            var list =  new List<CommandDataElement>()
            {
                new CommandDataElement("Root", -1, -1, new Mmogf.Core.Networking.DataBucket() { Bytes = -1, Messages = -1, }),
                //new CommandDataElement("Sample Command 2", 0, 102, new Mmogf.Core.Networking.DataBucket() { Bytes = 1210, Messages = 4, }),

            };

            if(_selectedConnection != null && _selectedConnection.ReceivedStats != null)
            {
                
                var stats = _selectedConnection.ReceivedStats;
                if(!received)
                    stats = _selectedConnection.SentStats;

                switch(_selectedStat)
                {
                    case DataStat.Command:
                        foreach (var stat in stats.CurrentTimeSlice.Commands)
                        {
                            var command = ComponentMappings.GetCommandType(stat.Key);
                            var el = new CommandDataElement(command?.Name ?? "", 0, stat.Key, stat.Value);
                            list.Add(el);
                        }
                        break;
                    case DataStat.Event:
                        foreach (var stat in stats.CurrentTimeSlice.Events)
                        {
                            var command = ComponentMappings.GetEventType(stat.Key);
                            var el = new CommandDataElement(command?.Name ?? "", 0, stat.Key, stat.Value);
                            list.Add(el);
                        }
                        break;
                    case DataStat.Update:
                        foreach (var update in stats.CurrentTimeSlice.Updates)
                        {
                            var component = ComponentMappings.GetComponentType(update.Key);
                            var el = new CommandDataElement(component?.Name ?? "", 0, update.Key, update.Value);
                            list.Add(el);
                        }
                        break;
                    case DataStat.Entity:
                        var entitySum = stats.CurrentTimeSlice.Sum(stats.CurrentTimeSlice.Entities);

                        var entitySumEl = new CommandDataElement("Entities", 0, 0, entitySum);
                        list.Add(entitySumEl);

                        break;
                    case DataStat.Summary:
                        var updatesSummary = stats.CurrentTimeSlice.Sum(stats.CurrentTimeSlice.Updates);
                        var uptSumEl = new CommandDataElement("Updates", 0, 0, updatesSummary);
                        list.Add(uptSumEl);
                        var commandsSummary = stats.CurrentTimeSlice.Sum(stats.CurrentTimeSlice.Commands);
                        var comSumEl = new CommandDataElement("Commands", 0, 0, commandsSummary);
                        list.Add(comSumEl);
                        var eventsSummary = stats.CurrentTimeSlice.Sum(stats.CurrentTimeSlice.Events);
                        var evnySumEl = new CommandDataElement("Events", 0, 0, eventsSummary);
                        list.Add(evnySumEl);                        
                        var entitySummary = stats.CurrentTimeSlice.Sum(stats.CurrentTimeSlice.Entities);
                        var entSumEl = new CommandDataElement("Entities", 0, 0, entitySummary);
                        list.Add(entSumEl);

                        var totalSummary = stats.CurrentTimeSlice.Sum(new[] { updatesSummary, commandsSummary, eventsSummary, entitySummary, });
                        var totalSumEl = new CommandDataElement("Totals", 0, 0, totalSummary);
                        list.Add(totalSumEl);

                        break;
                }

                
            }

            return list;

        }


        void OnGUI()
        {
            if (!processInitialized || _connections == null || _connections.Length == 0)
            {
                GUILayout.Label("No servers running.");
                Init();
                return;
            }

            EditorGUI.LabelField(new Rect(10, 5, 120, 20), "Received");
            if(EditorGUI.DropdownButton(new Rect(125, 5, 250, 20), new GUIContent($"Connection: {(_selectedConnection != null ? _selectedConnection.WorkerType : "------")}"), FocusType.Keyboard))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();
                if (_connections != null)
                {
                    foreach (var connection in _connections)
                    {
                        if (connection == null)
                            continue;
                        AddMenuItemForConnection(menu, connection.WorkerType, connection);
                    }

                    if (menu.GetItemCount() == 0)
                    {
                        AddMenuItemForConnection(menu, "-----", null);
                    }

                }

                // display the menu
                menu.ShowAsContext();
            }
            if (EditorGUI.DropdownButton(new Rect(380, 5, 150, 20), new GUIContent($"Type: {_selectedStat}"), FocusType.Keyboard))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();

                var enumValues = Enum.GetValues(typeof(DataStat));

                foreach(var val in enumValues)
                {
                    var dataStat = (DataStat)val;
                    AddMenuItemForData(menu, dataStat.ToString(), dataStat);
                }

                // display the menu
                menu.ShowAsContext();
            }
            _treeReceivedView.OnGUI(_treeViewRect);

            EditorGUI.LabelField(new Rect(10, _treeViewRect.height + _treeViewRect.y + 5, position.width - 20, 20), "Sent");


            _treeSentView.OnGUI(_sentViewRect);

        }

        public void OnInspectorUpdate()
        {
            if(!EditorApplication.isPlaying)
                return;

            _treeReceivedView.treeModel.SetData(GetData(true));
            _treeReceivedView.Reload();

            _treeSentView.treeModel.SetData(GetData(false));
            _treeSentView.Reload();

            // This will only get called 10 times per second.
            Repaint();
        }

        void AddMenuItemForConnection(GenericMenu menu, string menuPath, CommonHandler connection)
        {
            // the menu item is marked as selected if it matches the current value of m_Color
            menu.AddItem(new GUIContent(menuPath), _selectedConnection == connection, OnConnectionSelected, connection);
        }

        // a method to simplify adding menu items
        void AddMenuItemForData(GenericMenu menu, string menuPath, DataStat dataStat)
        {
            // the menu item is marked as selected if it matches the current value of m_Color
            menu.AddItem(new GUIContent(menuPath), _selectedStat == dataStat, OnDataStatSelected, dataStat);
        }

        // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
        void OnDataStatSelected(object dataStat)
        {
            _selectedStat = (DataStat)dataStat;
        }

        void OnConnectionSelected(object connection)
        {
            if(connection == null)
            {
                _selectedConnection = null;
                return;
            }

            _selectedConnection = (CommonHandler)connection;
        }

    }

}

#endif

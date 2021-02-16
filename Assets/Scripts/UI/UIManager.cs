using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProductionPipeline;

public class UIManager : MonoBehaviour
{    
    /// <summary>
    /// Dropdown for the selection of the Type of module (Assembler, Buffer, ecc).
    /// </summary>
    [SerializeField]
    private Dropdown _typesModulesDropdown;

    /// <summary>
    /// Dropdown for the names of all the modules of the chosen type.
    /// </summary>
    [SerializeField]
    private Dropdown _modulesNamesDropdown;

    /// <summary>
    /// Slider for the management of the time scale.
    /// </summary>
    [SerializeField]
    private Slider _timeSlider;

    /// <summary>
    /// Toggle for setting the simulation on play/pause.
    /// </summary>
    [SerializeField]
    private Toggle _pauseToggle;

    /// <summary>
    /// Sources panel.
    /// </summary>
    [SerializeField]
    private CanvasGroup _sourcesPanel;

    /// <summary>
    /// Dropdown for the selection of the Type of module (Assembler, Buffer, ecc).
    /// </summary>
    [SerializeField]
    private Dropdown _sourcesInPipelineDropdown;

    /// <summary>
    /// Text element for showing module stats.
    /// </summary>
    [SerializeField]
    private Text _moduleStatsText;

    /// <summary>
    /// Text element for showing module stats.
    /// </summary>
    [SerializeField]
    private Text _sourceStatsText;

    private CanvasGroup _canvasGroup;
    private PipelineManager _pipelineManager { get { return PipelineManager.Instance; } }


    private void OnEnable()
    {
        _pipelineManager.PipelineObjectsLoaded += _pipelineManager_PipelineObjectsLoaded;
        _pipelineManager.ChangedSourceData += _pipelineManager_ChangedSourceData;
        _pipelineManager.ChangedModuleData += _pipelineManager_ChangedModuleData;
        _pipelineManager.MouseClickedModule += Instance_MouseClickedModule;
        _pipelineManager.MouseClickedSource += Instance_MouseClickedSource;
    }

    private void OnDisable()
    {
        if (_pipelineManager != null)
        {
            _pipelineManager.PipelineObjectsLoaded -= _pipelineManager_PipelineObjectsLoaded;
            _pipelineManager.ChangedSourceData -= _pipelineManager_ChangedSourceData;
            _pipelineManager.ChangedModuleData -= _pipelineManager_ChangedModuleData;
            _pipelineManager.MouseClickedModule -= Instance_MouseClickedModule;
            _pipelineManager.MouseClickedSource -= Instance_MouseClickedSource;
        }
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Utilities.EnableCanvas(_canvasGroup, false);
    }

    /// <summary>
    /// Change the time scale of the simulation.
    /// </summary>
    public void SetTimeScale()
    {
        Time.timeScale = _timeSlider.value;
    }

    /// <summary>
    /// Pause the simulation.
    /// </summary>
    public void PauseSimulation()
    {
        _pipelineManager.SimulationIsPaused = _pauseToggle.isOn;
    }

    /// <summary>
    /// When the simulation is paused/played, the source panel change its interactivity.
    /// </summary>
    public void ToggleSourcesPanelInteractivity()
    {
        _sourcesPanel.interactable = !_sourcesPanel.interactable;
        _sourcesPanel.blocksRaycasts = !_sourcesPanel.blocksRaycasts;
        ShowSourceStats();
    }

    /// <summary>
    /// When the type of module is selected, show its modules and the statistics of the first selected.
    /// </summary>
    public void ShowModulesNames()
    {
        _modulesNamesDropdown.ClearOptions();
        _modulesNamesDropdown.AddOptions(_pipelineManager.GetModulesNames(_typesModulesDropdown.value));
        ShowModuleStats();
    }

    /// <summary>
    /// Shows the stats of the currently selected module.
    /// </summary>
    public void ShowModuleStats()
    {
        _moduleStatsText.text = _pipelineManager.GetModuleStats(_typesModulesDropdown.value, _modulesNamesDropdown.options[_modulesNamesDropdown.value].text);
    }

    /// <summary>
    /// Show the module stats.
    /// </summary>
    /// <param name="stats">Stats to be shown</param>
    private void ShowModuleStats(string stats)
    {
        _moduleStatsText.text = stats;
    }

    /// <summary>
    /// Show the currently selected sources stats.
    /// </summary>
    private void ShowSourceStats()
    {
        if (_sourcesInPipelineDropdown.value < _sourcesInPipelineDropdown.options.Count)
            _sourceStatsText.text = _pipelineManager.GetSourceStats(_sourcesInPipelineDropdown.options[_sourcesInPipelineDropdown.value].text);
    }

    /// <summary>
    /// Show the source stats.
    /// </summary>
    /// <param name="stats">Stats to be shown</param>
    private void ShowSourceStats(string stats)
    {
        _sourceStatsText.text = stats;
    }

    private void _pipelineManager_PipelineObjectsLoaded(object sender, System.EventArgs e)
    {
        List<string> modTypes = _pipelineManager.GetModuleTypesNames();
        _typesModulesDropdown.AddOptions(modTypes);
        _typesModulesDropdown.value = 0;
        ShowModulesNames();
        ShowModuleStats();
        Utilities.EnableCanvas(_canvasGroup, true);
    }

    /// <summary>
    /// Handles the event relative to a source.
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Params of the event</param>
    private void _pipelineManager_ChangedSourceData(object sender, ProductionPipeline.PipelineManager.SourceEventArgs e)
    {
        // if newStats == "", it means that the source must be removed
        if (!e.newSource && e.newStats == "")
        {
            string toBeRemoved = e.sourceId;
            for (int i = 0; i < _sourcesInPipelineDropdown.options.Count; i++)
            {
                if (_sourcesInPipelineDropdown.options[i].text == toBeRemoved)
                {
                    _sourcesInPipelineDropdown.options.RemoveAt(i);
                    break;
                }
            }
            ShowSourceStats();
        }
        // otherwise, it must be added or updated
        else
        {
            if (!e.newSource && _sourcesInPipelineDropdown.options[_sourcesInPipelineDropdown.value].text == e.sourceId)
            {
                ShowSourceStats(e.newStats);
            }
            if (e.newSource)
            {
                List<string> newId = new List<string>();
                newId.Add(e.sourceId);
                _sourcesInPipelineDropdown.AddOptions(newId);
            }
        }
        _sourcesInPipelineDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Handles the event relative to a module.
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Params of the event</param>
    private void _pipelineManager_ChangedModuleData(object sender, ProductionPipeline.PipelineManager.ModuleEventArgs e)
    {
        int moduleTypeEvent = (int)e.moduleType;
        if (_modulesNamesDropdown.options.Count > 0 && 
            moduleTypeEvent == _typesModulesDropdown.value && e.moduleName == _modulesNamesDropdown.options[_modulesNamesDropdown.value].text)
        {
            ShowModuleStats(e.newStats);
        }
    }

    /// <summary>
    /// Mouse clicked on a module.
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Params of the event</param>
    private void Instance_MouseClickedModule(object sender, PipelineManager.MouseModuleClickedArgs e)
    {
        _typesModulesDropdown.value = e.type;
        _typesModulesDropdown.RefreshShownValue();
        for (int i = 0; i < _modulesNamesDropdown.options.Count; i++)
        {
            if (_modulesNamesDropdown.options[i].text == e.name)
            {
                _modulesNamesDropdown.value = i;
                _modulesNamesDropdown.RefreshShownValue();
                break;
            }
        }
    }

    /// <summary>
    /// Mouse clicked on a source.
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Params of the event</param>
    private void Instance_MouseClickedSource(object sender, PipelineManager.MouseSourceClickedArgs e)
    {
        for (int i = 0; i < _sourcesInPipelineDropdown.options.Count; i++)
        {
            if (_sourcesInPipelineDropdown.options[i].text == e.id)
            {
                _sourcesInPipelineDropdown.value = i;
                _sourcesInPipelineDropdown.RefreshShownValue();
                ShowSourceStats();
                break;
            }
        }
    }

}

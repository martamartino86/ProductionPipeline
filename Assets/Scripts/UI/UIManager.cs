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
    

    private void OnEnable()
    {
        PipelineManager.Instance.PipelineObjectsLoaded += _pipelineManager_PipelineObjectsLoaded;
        PipelineManager.Instance.ChangedSourceData += _pipelineManager_ChangedSourceData;
        PipelineManager.Instance.ChangedModuleData += _pipelineManager_ChangedModuleData;
        PipelineManager.Instance.MouseClickedModule += Instance_MouseClickedModule;
        PipelineManager.Instance.MouseClickedSource += Instance_MouseClickedSource;
    }

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

    private void Instance_MouseClickedSource(object sender, PipelineManager.MouseSourceClickedArgs e)
    {
        for (int i = 0; i < _sourcesInPipelineDropdown.options.Count; i++)
        {
            if (_sourcesInPipelineDropdown.options[i].text == e.id)
            {
                _sourcesInPipelineDropdown.value = i;
                _sourcesInPipelineDropdown.RefreshShownValue();
                break;
            }
        }
    }


    private void OnDisable()
    {
        PipelineManager.Instance.PipelineObjectsLoaded -= _pipelineManager_PipelineObjectsLoaded;
        PipelineManager.Instance.ChangedSourceData -= _pipelineManager_ChangedSourceData;
        PipelineManager.Instance.ChangedModuleData -= _pipelineManager_ChangedModuleData;
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Utilities.EnableCanvas(_canvasGroup, false);
    }

    public void SetTimeScale()
    {
        Time.timeScale = _timeSlider.value;
    }

    public void PauseSimulation()
    {
        PipelineManager.Instance.SimulationIsPaused = _pauseToggle.isOn;
    }

    public void ToggleSourcesPanelInteractivity()
    {
        _sourcesPanel.interactable = !_sourcesPanel.interactable;
        _sourcesPanel.blocksRaycasts = !_sourcesPanel.blocksRaycasts;
        ShowSourcesStats();
    }

    /// <summary>
    /// When the type of module is selected, show its modules and the statistics of the first selected.
    /// </summary>
    public void ShowModulesNames()
    {
        _modulesNamesDropdown.ClearOptions();
        _modulesNamesDropdown.AddOptions(PipelineManager.Instance.GetModulesNames(_typesModulesDropdown.value));
        ShowModuleStats();
    }

    /// <summary>
    /// Quando ho selezionato il nome di un modulo, mostro le sue stats
    /// </summary>
    /// <param name="moduleName"></param>
    public void ShowModuleStats()
    {
        _moduleStatsText.text = PipelineManager.Instance.GetModuleStats(_typesModulesDropdown.value, _modulesNamesDropdown.options[_modulesNamesDropdown.value].text);
    }
    private void ShowModuleStats(string stats)
    {
        _moduleStatsText.text = stats;
    }

    private void ShowSourcesStats()
    {
        if (_sourcesInPipelineDropdown.value < _sourcesInPipelineDropdown.options.Count)
            _sourceStatsText.text = PipelineManager.Instance.GetSourceStats(_sourcesInPipelineDropdown.options[_sourcesInPipelineDropdown.value].text);
    }
    private void ShowSourcesStats(string stats)
    {
        _sourceStatsText.text = stats;
    }

    private void _pipelineManager_PipelineObjectsLoaded(object sender, System.EventArgs e)
    {
        List<string> modTypes = PipelineManager.Instance.GetModuleTypesNames();
        _typesModulesDropdown.AddOptions(modTypes);
        _typesModulesDropdown.value = 0;
        ShowModulesNames();
        ShowModuleStats();
        Utilities.EnableCanvas(_canvasGroup, true);
    }

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
            ShowSourcesStats();
        }
        // otherwise, it must be added or updated
        else
        {
            if (!e.newSource && _sourcesInPipelineDropdown.options[_sourcesInPipelineDropdown.value].text == e.sourceId)
            {
                ShowSourcesStats(e.newStats);
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


    private void _pipelineManager_ChangedModuleData(object sender, ProductionPipeline.PipelineManager.ModuleEventArgs e)
    {
        int moduleTypeEvent = (int)e.moduleType;
        if (_modulesNamesDropdown.options.Count > 0 && 
            moduleTypeEvent == _typesModulesDropdown.value && e.moduleName == _modulesNamesDropdown.options[_modulesNamesDropdown.value].text)
        {
            ShowModuleStats(e.newStats);
        }
    }


}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private ProductionPipeline.PipelineManager _pipelineManager;
    
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
    /// Text element for showing stats.
    /// </summary>
    [SerializeField]
    private Text _statsText;

    private CanvasGroup _canvasGroup;
    

    private void OnEnable()
    {
        _pipelineManager.PipelineObjectsLoaded += _pipelineManager_PipelineObjectsLoaded;
        _pipelineManager.ChangedData += _pipelineManager_ChangedData;
    }

    private void OnDisable()
    {
        _pipelineManager.PipelineObjectsLoaded -= _pipelineManager_PipelineObjectsLoaded;
        _pipelineManager.ChangedData -= _pipelineManager_ChangedData;
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
        Time.timeScale = _pauseToggle.isOn ? 0 : 1;
    }

    /// <summary>
    /// When the type of module is selected, show its modules and the statistics of the first selected.
    /// </summary>
    public void ShowModulesNames()
    {
        _modulesNamesDropdown.ClearOptions();
        _modulesNamesDropdown.AddOptions(_pipelineManager.GetModulesNames(_typesModulesDropdown.value));
        ShowStats();
    }

    /// <summary>
    /// Quando ho selezionato il nome di un modulo, mostro le sue stats
    /// </summary>
    /// <param name="moduleName"></param>
    public void ShowStats()
    {
        _statsText.text = _pipelineManager.GetStats(_typesModulesDropdown.value, _modulesNamesDropdown.options[_modulesNamesDropdown.value].text);
    }

    private void ShowStats(string stats)
    {
        _statsText.text = stats;
    }

    private void _pipelineManager_PipelineObjectsLoaded(object sender, System.EventArgs e)
    {
        List<string> modTypes = _pipelineManager.GetModuleTypesNames();
        _typesModulesDropdown.AddOptions(modTypes);
        _typesModulesDropdown.value = 0;
        ShowModulesNames();
        ShowStats();
        Utilities.EnableCanvas(_canvasGroup, true);
    }

    private void _pipelineManager_ChangedData(object sender, ProductionPipeline.PipelineManager.DataEventArgs e)
    {
        int moduleTypeEvent = (int)e.moduleType;
        if (_modulesNamesDropdown.options.Count > 0 && 
            moduleTypeEvent == _typesModulesDropdown.value && e.moduleName == _modulesNamesDropdown.options[_modulesNamesDropdown.value].text)
        {
            ShowStats(e.newStats);
        }
    }


}

using PROTOCOL;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using FPS;

namespace Settings
{
    /// <summary>
    /// Форма для редактирования параметров приложения
    /// </summary>
    public partial class FRSettings : Form
    {
        Settings settings;
        /// <summary>
        /// Конструктор
        /// </summary>
        public FRSettings()
        {
            InitializeComponent();
            //pg.PropertySort = PropertySort.NoSort;
            pg.PropertySort = PropertySort.Categorized;
            settings = AppSettings.settings;
            pg.PropertyValueChanged += Pg_PropertyValueChanged;
            pg.SelectedObject = settings;
        }

        private void Pg_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: Изменено {2}: {3}=>{4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.ChangedItem.Label, e.OldValue, e.ChangedItem.Value);
            if (settings.onChangeSettings != null) settings.onChangeSettings(new object[] { e.ChangedItem.Label, e.ChangedItem.Value });
            settings.changed = true;
        }

        private void miSaveSettings_Click(object sender, EventArgs e)
        {
            Settings.save(AppSettings.settings);
        }

        private void FRSettings_Load(object sender, EventArgs e)
        {
            FormPosSaver.load(this);
        }

        private void FRSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.save(AppSettings.settings);
            FormPosSaver.save(this);
        }
    }
}

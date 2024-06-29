using System;
using System.Drawing;
using System.Windows.Forms;
using Data;
using PROTOCOL;
using CHART;

namespace USPC
{
    partial class FRDetails : Form
    {
        USPCData data;
        int zone;
        int sensor;
        ChartCursorColumn measCur;
        MeasChart measChart;

        public FRDetails(Form _owner,USPCData _data, int _zone = 0, int _sensor = 0, int _offset = 0)
        {
            Owner = _owner;
            InitializeComponent();
            KeyPreview = true;

            data = _data;
            zone = _zone;
            sensor = _sensor;
            //offset = _offset;

            //Настройка чарта с измерениями по зоне
            measChart = new MeasChart();
            measChart.Init(this, sbMeas.Height+3, data, zone, sensor, _offset, onMeasCursorMove);
            measCur = measChart.curMeas;
            putDataOnChart();
        }

        private void putDataOnChart()
        {
            //measChart.putDataOnChart(data.evalZone(zone, sensor, cbMedian.Checked));
            //measChart.putColorDecision(data, zone, sensor);
            sbMeas.Items[0].Text = string.Format("Зона: {0}", zone);
            sbMeas.Items[1].Text = string.Format("Датчик: {0}", sensor + 1);
            sbMeas.Items[2].Text = string.Format("Измерение: {0}", measCur.x);
            sbMeas.Items[3].Text = string.Format("Толщина: {0}", measCur.y);
            //!!! CORRECT
            sbMeas.Items[5].Text = string.Format("Количество измерений в зоне: {0}", (Program.data[0].offsets[zone+1]-Program.data[0].offsets[zone])/USPCData.countSensors);
            Text = string.Format("Детализированный просмотр трубы: зона {0}, датчик {1}, измерение {2}", zone, sensor + 1, measCur.x);
        }
        private void onMeasCursorMove(int pos)
        {
            putDataOnChart();
        }

        private void FRTubeDetails_KeyDown(object sender, KeyEventArgs e)
        {
            switch( e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    e.Handled = true;
                    break;
                case Keys.Down:
                    if (sensor < USPCData.countSensors-1) sensor++;
                    else sensor = 0;
                    putDataOnChart();
                    break;
                case Keys.Up:
                    if (sensor > 0) sensor--;
                    else sensor = USPCData.countSensors - 1;
                    putDataOnChart();
                    break;
                case Keys.Left:
                    if (e.Alt)
                    {
                        if (zone > 0) zone--;
                        else zone = USPCData.countZones - 1;
                        putDataOnChart();
                        e.Handled = true;
                    }
                    break;
                case Keys.Right:
                    if (e.Alt)
                    {
                        if (zone < USPCData.countZones - 1) zone++;
                        else zone = 0;
                        putDataOnChart();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void cbMedian_CheckedChanged(object sender, EventArgs e)
        {
            putDataOnChart();
        }
    }
}

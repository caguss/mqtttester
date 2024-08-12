using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace mqtttester
{
    public partial class Form1 : Form
    {
        public MqttClient mqttClient;
        private static string CLIENT_ID = Guid.NewGuid().ToString() + "_MQTT_Sender";

        public Form1()
        {
            InitializeComponent();
        }
     

        /// <summary>
        /// 구독한 Topic에 대한 메시지 수신 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Writelog("MQTT Received");
            Writelog($"Topic : {e.Topic} - Message : {System.Text.Encoding.UTF8.GetString(e.Message)}");
        }

        private async void Writelog(string log)
        {
            if (logbox.InvokeRequired)
            {
                logbox.Invoke(new Action(() => logbox.Items.Insert(0, $"[{DateTime.Now}]{log}")));
            }
            else
            {
                logbox.Items.Insert(0, $"[{DateTime.Now}]{log}");
            }
        }

        private void txt_Action_Click(object sender, EventArgs e)
        {
            //MQTT로 데이터 전송
            Writelog($"MQTT Send : {txtSendText.Text}");
            mqttClient.Publish($"MQTTTEST/{DateTime.Now.ToString("yyyyMMdd")}", System.Text.Encoding.UTF8.GetBytes(txtSendText.Text), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SubscribeStart();
        }

        /// <summary>
        /// 원하는 MQTT Topic을 구독하는 메서드
        /// </summary>
        internal async void SubscribeStart()
        {
            //MQTT로 데이터 수집
            try
            {
                mqttClient = new MqttClient($"{txtIP.Text}");
                if (!mqttClient.IsConnected)
                {
                    mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                    mqttClient.Subscribe(new string[] { "MQTTTEST/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); //DMS Topic : event/c/data_collection_digit/#

                    mqttClient.Connect($"{txtIP.Text}:{txtPort.Text}");
                }
                Writelog("MQTT Subscribe Start");

            }
            catch (Exception ex)
            {
                Writelog(ex.Message);
            }

        }
    }
}

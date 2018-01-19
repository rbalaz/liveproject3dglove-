/* 3D Glove
 Left Hand programm
 by 110mat110 for Siemens Healthineers
 This code is under copyright.

 Created 10 January 2018
 Version 1.0.

 Technical University Kosice
*/ 

#include <Servo.h> 
#include <ESP8266WiFi.h>
#include <EEPROM.h>

Servo servoList[5]; 
   
const int buttonPin = 2; 
const char* ssid     = "SSID";
const char* password = "Password1";

char* host = "192.168.xxx.xxx";
const int Port = 7999;
WiFiClient client;

bool InputDone = false;
bool ServoFreeState = false;
String output = "";
bool wifiReferenceSet = false;
bool stringComplete = false;           
String inputString = "";  
int vcounter = 0;
  
void setup() 
{
  //Initialize
  RGBOff();
  pinMode(buttonPin, INPUT);
  
  for(int i=0;i<5;i++)
    servoList[i].write(100);

  //loadCredentials();
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) {
    G_ON();
    delay(300);
    G_OFF();
    delay(300);
  }
  
  if(!client.connect(host, Port))
  {
    for (int i=0; i<3; i++){
      R_ON();
      delay(100);
      R_OFF();
      delay(100);
    }
    ESP.restart();
  }
  G_ON();
  
  client.println("l");

  servoList[0].attach(16);
  servoList[1].attach(14);
  servoList[2].attach(12);
  servoList[3].attach(13);
  servoList[4].attach(15);
} 
 
void loop() { 
  if (stringComplete){
    //TODO read and save WIFI status;
  }
  if (!ServoFreeState){
    if(client.available()){
      String line = "";
      line += client.readStringUntil('y');
      for (int i=0; i<line.length(); i++){
          output += line[i];
          if (line[i]=='x') 
            InputDone = true; 
      }
    }
      
    if (InputDone){
      InputDone = false;
      HandleServo(output);  
      output = "";
      delay(10);
    }
  }

  if (digitalRead(buttonPin) == LOW){
    ServoFreeState = !ServoFreeState;
    if (ServoFreeState){
      HandleServo("120,120,120,120,120x"); 
      R_ON();
      delay(100);
      while (digitalRead(buttonPin) == LOW) ;
      delay(50);
      servoList[0].detach();
      servoList[1].detach();
      servoList[2].detach();
      servoList[3].detach();
      servoList[4].detach();
    }else{
      G_ON();   
      servoList[0].attach(16);
      servoList[1].attach(14);
      servoList[2].attach(12);
      servoList[3].attach(13);
      servoList[4].attach(15);
      delay(100);
      while (digitalRead(buttonPin) == LOW) ;
      delay(50);
    }
  }
  
  if (vcounter>10000){
    vcounter = 0;
    if (ReadVoltage() < 370){
      R_ON();
      delay(70);
      R_OFF();
      delay(30);
    }
  }
  vcounter ++;
}

/* 
void serialEvent() {
    if (!client.connected()){
      while (Serial.available()) {
      char inChar = (char)Serial.read();

      if (inChar == '\n') {
        stringComplete = true;
      } else{
        inputString += inChar;
      }
    }
  }
}
*/
void loadCredentials() {
  EEPROM.begin(512);
  EEPROM.get(0, ssid);
  EEPROM.get(0+sizeof(ssid), password);
  char ok[2+1];
  EEPROM.get(0+sizeof(ssid)+sizeof(password), host);
  EEPROM.get(0+sizeof(ssid)+sizeof(password)+sizeof(host), ok);
  EEPROM.end();
  if (String(ok) != String("OK")) {
      ssid     = "DefaultSSID";
      password = "DefaultPassword";
      host = "192.168.xxx.xxx";
  }
}

void HandleServo(String o){
      B_ON();
      int j=0;
      for (int i=0; i<5; i++){ 
        String help = "";
        int num=0;
        while(o[j]!='x' && o[j]!=','){   
          help += o[j];
          j++;
        }
        
        num = help.toInt();
        if (num < 60) num = 60;
        if (num > 120) num = 120;
        servoList[i].write(num);
        j++;
      }
      B_OFF();
}

void R_ON(){
    RGBOff();
    pinMode(1, OUTPUT);
    digitalWrite(1, HIGH);
  }
void R_OFF(){
    pinMode(1, OUTPUT);
    digitalWrite(1, LOW);
  }
  void G_ON(){
    RGBOff();
    pinMode(0, OUTPUT);
    digitalWrite(0, HIGH);
  }
void G_OFF(){
    pinMode(0, OUTPUT);
    digitalWrite(0, LOW);
  }
  void B_ON(){
    RGBOff();
    pinMode(3, OUTPUT);
    digitalWrite(3, HIGH);
  }
void B_OFF(){
      pinMode(3, OUTPUT);
    digitalWrite(3, LOW);
  }

void RGBOff(){
  R_OFF();
  G_OFF();
  B_OFF();
}

int ReadVoltage(){
    int voltage = analogRead(A0);
    voltage = voltage / 1.5;
    return voltage;
  }

#include <Servo.h>

Servo servo1;
int servoPos = 0;

const int MAX_BUFFER_LEN = 30;
char rcvBuffer[MAX_BUFFER_LEN];
bool newData = false;

void setup()
{
    servo1.attach(9);
    servo1.write(0);
    Serial.begin(115200);
    Serial.setTimeout(100);
}

void loop()
{
    receiveData();
    controlServo();
}

void receiveData()
{
    static int i = 0;
    char endTag = '#';

    char rcv;
    while (Serial.available() > 0 && !newData) 
    {
        rcv = Serial.read();
      /*  Serial.print("rcv: ");
        Serial.println(rcv);*/

        if (rcv != endTag)
        {
            rcvBuffer[i] = rcv;
            i++;
            if (i >= MAX_BUFFER_LEN)
            {
                i = MAX_BUFFER_LEN - 1;
            }
        }
        else
        {
            rcvBuffer[i] = '\0';
            i = 0;
            newData = true;
            /*Serial.print("rcvBuffer: ");
            Serial.println(rcvBuffer);
            Serial.print("i: ");
            Serial.println(i);*/
        }
    }
}

void controlServo()
{
    if (newData == true)
    {
        int incPos = atoi(rcvBuffer);
       /* Serial.print("incpos: ");
        Serial.println(incPos);*/
        servoPos = constrain(incPos, 0, 164);
        servo1.write(servoPos);
        newData = false;
    }
}


#include <VarSpeedServo.h>
const int SV_PIN = 6;
VarSpeedServo servo;

const int INIT = 180;
const int UP = 90;
const int DOWN = UP - 19;

void setup() {
  servo.attach(SV_PIN, 500, 2400);
  servo.write(UP, 0, true);
  
  Serial.begin(9600);
}

void loop() {
  serial_update();
}

void serial_update() {
  if (!Serial.available()) return;
  Serial.println("data available");
  char c = Serial.read();
  Serial.println("read");
  servo_switch(c);
  Serial.println("update completed");
}

void servo_switch(char c) {
  switch (c) {
    case 'd':
      Serial.println("down received");
      servo.write(DOWN, 20, true);
      Serial.println("down wrote");
      break;
    case 'u':
      Serial.println("up received");
      servo.write(UP, 0, true);
      Serial.println("up wrote");
      break;
    default:
      Serial.println("ignore");
      break;
  }
  delay(50);
}

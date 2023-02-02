#include "Arduino.h"

class Controller{
public:
  int pin;
  int id;
  bool enabled;    //Is the controller even able to connect (is the arduino pin connected to port)
  bool connected;  //Is the controller plugged in and responding
  bool rumble;
  byte data[8];
public:
  Controller(int p, int i){
    // p - pin controller is connected to (-1 if not connected), i - id of controller (index in controllers array)
    pin = p;
    id = i;
    enabled = p!=-1; 
    rumble = false;
    connected = false;
  }
};

#include "GameCube.cpp"

GameCube gc;
bool timeout = false;
unsigned long lastmillis = 0;

void setup() {
  Serial.begin(19200);
  gc.init(8, 9, 10, -1);
  delay(100);
  Serial.println("ready");

}

void loop() {

  //Wait for polling message from pc over serial
  //Message format: get<controller_id><rumble_status>-
  //<controller_id> 1, 2, 3 or 4 - port of gamecube
  //<rumble_status> 0 or 1 - turn rumble off or on
  String res = Serial.readStringUntil('-');
  Serial.flush();
  //Check if message is appropriate and extract rumble info
  int len = res.length();
  if(len>=5 ){
    if(res.substring(len-5, len-2) == "get"){
      lastmillis = millis();
      
      int cid = (res[len-2]-'0')-1; //Converts character '1'-'4' to int 0-3 representing controller id
      
      //Set rumble
      gc.setRumble(cid, res[len-1] == '1'); 
      
      //Get Controller state
      gc.getData(cid);
      
      //Send Controller state
      gc.sendData(cid);
    } 
    else {
      Serial.print("Bad message: ");
      Serial.print(res.substring(0, 3));
      Serial.println(res);
    }
  }

  //If a second has passed without recieving a message
  if(millis()-lastmillis >= 1000){
    lastmillis = millis();
    for(int i = 0; i<4; i++){
      gc.setRumble(i, false);
      gc.getData(i);
    }
    
  }

  
}

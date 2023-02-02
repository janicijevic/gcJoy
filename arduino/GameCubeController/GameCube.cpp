//GameCube module
#include "Controller.cpp"

//10 counts of TCNT0 means 40us
#define TIMEOUT 100

class GameCube{
private:
  Controller *conts[4];
    
public:
  GameCube(){}
  void init(int p1, int p2, int p3, int p4){
    //Initializes ports and calculates masks for each of the four controllers
    //TODO:
    //  make it run on all ports
    //  only clear necessary pins ie dont clear entire port
    PORTB = 0;                //Clear PORTB state
    DDRB = 0;                 //Make PORTB input
    MCUSR |= _BV(PUD);        //Disable internal pullups
    int _pins[4] = {p1, p2, p3, p4};
    int enabledCount = 0;
    for(int i = 0; i<4; i++){ 
      if(_pins[i] < 1 || _pins[i] > 12){
        //disabled controller
        this->conts[i] = new Controller(-1, -1);
      }
      else{
        this->conts[i] = new Controller(_pins[i], enabledCount);
        enabledCount++;
      }
      Serial.print(conts[i]->id); Serial.print(" ");
    }
  }

  void setRumble(int cid, bool rumble){
    conts[cid]->rumble = rumble;
  }

  void getData(int cid){
    //Gets data from controller and stores it in controller object
    if(conts[cid]->enabled){
      //Probe controller to make sure its still connected
      if(conts[cid]->connected = probe(conts[cid])){
        delay(2);
        //Send request for data to controller and store state
        request(conts[cid]);
        //Reset rumble
        conts[cid]->rumble = false;
      }
    }
  }
  
  void sendData(int cid){
    //Sends controller state over serial
    Controller *c = conts[cid];
    int stat = 0;
    if(!c->enabled) stat = 2;
    else if(!c->connected) stat = 1;

    //Send header with format: <controller id 2b><status 5b><parity bit 1b>
    //  status = 0 indicates controller is connected and the next 8 bytes are controller data + 1 checksum byte
    //  status = 1 indicates controller is currently not connected to port
    //  status = 2 indicates controller is disabled by driver
    byte header = (cid&0b11)<<6;     // Controller id
    header |= (stat&0b11111)<<1;        // Status
    header |= findParity(header); // Parity 

    Serial.write(header);
    
    if(stat == 0){
      // Send payload
      int numOfBytes = 0;
      byte checkSum = 0;
      for(numOfBytes = 0; numOfBytes<8; numOfBytes++){
        Serial.write(c->data[numOfBytes]);
        checkSum ^= c->data[numOfBytes];
      }
      Serial.write(checkSum);
    }
  }

  bool findParity(byte x){
    byte res = 0;
    while(x!=0){
      x = x&(x-1);
      res ^= 1;
    }
    return res&1;
  }

  bool probe(Controller *c){
    //Send probe signal to controller and check if it responds, 
    //returns true if controller responds
    int p = c->pin;
    byte a = 0;
    byte mask = digitalPinToBitMask(p);
    byte portNum = digitalPinToPort(p);
    volatile byte *port = portModeRegister(portNum);  //Data direction register
    volatile byte *in = portInputRegister(portNum);   //Input register

    //Stop interrupts
    asm("cli");

    //Send probe signal -> 0000 0000 1
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendOne(port, mask);

    __builtin_avr_delay_cycles(7);  //Wait for controller to respond

    a = *in & mask; //check if controller pulled line to low

    //Start interrupts
    asm("sei");
    
    return a==0; //True if controller pulled low
  }

  bool request(Controller *c){
    //Sends specific signal to controller, reads its response and saves it to controller object
    int i = 0, p = c->pin;
    byte numOfBits = 0, numOfBytes = 0, tmp = 0;
    byte data[8];
    byte mask = digitalPinToBitMask(p);
    byte portNum = digitalPinToPort(p);
    volatile byte *port = portModeRegister(portNum);  //Data direction register
    volatile byte *in = portInputRegister(portNum);   //Input register
    
    //Stop interrupts
    asm("cli");

    //Send request message to get data
    if(c->rumble){
      sendMessageRumble(port, mask);
    }
    else{
      sendMessage(port, mask);
    }

    //Read response
    while(numOfBytes < 8){
      numOfBits = 0;
      while(numOfBits < 8){
        tmp = *in & mask;         //Get state of data pin
        TCNT0 = 0;                //Clear timer0
        while(tmp != 0){          //Wait for drop
          tmp = *in & mask;       //Get state of data pin
          if(TCNT0 > TIMEOUT){    //The timeout period has passed and no drop has been detected
            disconnected(c);      //Controller not responding to message
            return 0;
          }
        };                    
        __builtin_avr_delay_cycles(18);   //Wait ~1.5us
        data[numOfBytes] = (data[numOfBytes] << 1 ) + ((*in & mask)!=0);  //Append bit to data
        numOfBits++;             
        tmp = *in & mask;         //Get state of data pin
        TCNT0 = 0;                //Clear timer0       
        while(tmp == 0){          //Wait for rise    
          tmp = *in & mask;       //Get state of data pin
          if(TCNT0 > TIMEOUT){    //The timeout period has passed and no drop has been detected
            disconnected(c);
            return 0;
          }
        };                    
        __builtin_avr_delay_cycles(6);   //Wait for signal to stabilize
      }
      numOfBytes++;
    }

    //Start interrupts
    asm("sei");

    //Save response
    for(numOfBytes = 0; numOfBytes<8; numOfBytes++){
      c->data[numOfBytes] = data[numOfBytes];
    }
    return 1;
  }

  void disconnected(Controller *c){
    //Handles controller being unplugged while reading response
    //Start interrupts
    asm("sei");
    Serial.println("disconnected-");
  }

  
  void sendMessage(volatile byte* port, byte mask){
    //0100 0000 0000 0011 0000 0010 1
    sendZero(port, mask);sendOne(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendOne(port, mask);sendOne(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendOne(port, mask);sendZero(port, mask);
    sendStop(port, mask);
  }
  
  void sendMessageRumble(volatile byte* port, byte mask){
    //0100 0000 0000 0011 0000 0001 1
    sendZero(port, mask);sendOne(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendOne(port, mask);sendOne(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);
    sendZero(port, mask);sendZero(port, mask);sendZero(port, mask);sendOne(port, mask);
    sendStop(port, mask);
  }

  void sendStop(volatile byte* port, byte mask){
    //Send 1us low, 2us high  _--
    //DDRB |= masks[p];
    *port |= mask;
    __builtin_avr_delay_cycles(14);
    //DDRB &= 0b1111111 ^ masks[p];
    //1110111 0111
    //DDRB &= 0b11111111 ^ masks[p];
    *port &= ~mask;
    __builtin_avr_delay_cycles(28);
    return;
  }
  void sendOne(volatile byte* port, byte mask){
    //Send 1us low, 3us high  _---
    *port |= mask;
    __builtin_avr_delay_cycles(14);
    //DDRB &= 0b1111110;
    *port &= ~mask;
    __builtin_avr_delay_cycles(44);
    return;
  }
  void sendZero(volatile byte* port, byte mask){
    //Send 3us low, 1us high  ___-
    *port |= mask;
    __builtin_avr_delay_cycles(45);
    *port &= ~mask;
    __builtin_avr_delay_cycles(12);
    return;
  }

    
  
};

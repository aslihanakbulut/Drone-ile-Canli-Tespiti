#include <NewPing.h>
#include <Wire.h>
#include <Adafruit_MLX90614.h>
#include <TinyGPS.h>
#include <SoftwareSerial.h>
#include <string.h>

#define TRIGGER_PIN 8
#define ECHO_PIN 9
#define maks_mesafe 500

TinyGPS konum;
SoftwareSerial bt(5, 6);
NewPing sonar(TRIGGER_PIN, ECHO_PIN, maks_mesafe);
Adafruit_MLX90614 mlx = Adafruit_MLX90614();

float enlem, boylam;
unsigned long gecenSure1;
float mesafe;
float sicaklik, okunanDeger;
float toplam;
int i = 0;

int led[] = { 2, 3, 4, 7 };
int ledSayisi = 4;

String voice;  // Gönderdiğimiz sesleri algılaması için voice değişkenini atadık.
int sesDurum;



void setup() {

  Serial.begin(9600);  // Seri haberlesmeyi baslat
  mlx.begin();
  bt.begin(9600);

  for (int ledPin = 0; ledPin < ledSayisi; ledPin++) {
    pinMode(led[ledPin], OUTPUT);
  }
}

void loop() {

  Sesli_komut();
  //Konum_al();
}

void Sesli_komut() {
  while (bt.available()) {  //Bluetooth iletisimini basladi mi kontrol et
    delay(10);
    char c = bt.read();  // char ile gelen degeri oku
    if (c == '#') { break; }
    voice += c;
  }


  if (voice.length() > 0) {

    //Serial.println(voice);  // speech to text

    if (voice == "ölç") {  //

      sesDurum = 1;

      while (sesDurum) {
        Konum_al();
        Sicaklik_olc();
        mesafe = sonar.ping_cm();  // ping gondererek cm olarak mesafe olc


        Serial.print(mesafe);  // seri terminalde degeri yazdir


        if (mesafe > 200) {  // mesafe 2m olduysa ledleri yak
          for (int i = 0; i < ledSayisi; i++) {
            digitalWrite(led[i], HIGH);
            delay(500);
            digitalWrite(led[i], LOW);
          }
        }

        //delay(500);
      }
    }


    voice = "";
  }
}

void Sicaklik_olc() {
  while (i < 30) {

    okunanDeger = mlx.readObjectTempC();
    toplam += okunanDeger;
    i++;
    delay(1);
  }
  sicaklik = toplam / 30.0;
  toplam = 0;
  i = 0;

  bt.print(sicaklik);
  bt.print("|");
  Serial.print("/");
  Serial.println(sicaklik);

  if (sicaklik > 24 && sicaklik < 30) {
    bt.print("algılandı");
    bt.print("|");
    //konum bilgilerini telefona gonder
    bt.print(enlem, 6);
    bt.print("|");
    bt.print(boylam, 6);
    bt.print("|");

    // konum bilgilerini bilgisayara gonder
    Serial.print("/");
    Serial.println(enlem, 6);  //virgulden sonra 6 basamak yaz
    Serial.print("/");
    Serial.println(boylam, 6);
  }

}

void Konum_al() {

  beklemeSuresi(1000);
  Serial.println();

  uint8_t uydu = konum.satellites();
  
  konum.f_get_position(&enlem, &boylam, &gecenSure1);

  int rakim = konum.f_altitude();
S
  int yil;
  byte ay, gun, saat, dakika, saniye, hundredths;
  unsigned long gecenSure2;
  konum.crack_datetime(&yil, &ay, &gun, &saat, &dakika, &saniye, &hundredths, &gecenSure2);

}


static void beklemeSuresi(unsigned long sure) {
  unsigned long baslangic = millis();
  do {
    while (Serial.available())
      konum.encode(Serial.read());
  } while (millis() - baslangic < sure);
}

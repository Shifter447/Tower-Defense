# Sprint 0 - Game Design Document : Tower Defense  

**Naam:** Tim Besdemiotis  
**Klas:** SD2A  
**Datum:** 12-9-2025  

---

## 1. Titel en elevator pitch  
**Titel:** Mech Fortress  

**Elevator pitch:**  
Bouw futuristische torens om je basis te verdedigen tegen golven van robotvijanden. Door slim energie te beheren en torens strategisch te upgraden, blijft elke wave een uitdaging.  

---

## 2. Wat maakt jouw tower defense uniek  
Spelers moeten **scrap** gebruiken omtorens te bouwen. Scrap can je krijgen door enemies te verslaan.  

---

## 3. Schets van je level en UI  
<img width="1009" height="658" alt="Schermafbeelding 2025-09-12 095438" src="https://github.com/user-attachments/assets/c9d2ca75-c985-41c9-93c6-dfb97442f516" />
  

De schets bevat:  
- Een zigzag-pad van startpunt.  
- Plaatsen naast het pad voor torens (vierkante bouwplaatsen).  
- De basis).  
- UI met: Scrap, wave teller, levens, startknop, pauzeknop.  

**Legenda:**  
- 🔴 = vijanden  
- 🔵 = pad  
- ⚫ = torens  
- 🟡 = basis/reactor  
- ⚪ = UI elementen  

---

## 4. Torens  
- **Machine Gun**  
  - Bereik: groot  
  - Schade: gemiddeld  
  - Eigenschap: raakt meerdere vijanden achter elkaar (doordringend).  

- **Tesla Tower**  
  - Bereik: middel  
  - Schade: laag  
  - Eigenschap: vertraagt vijanden met een EMP-puls.  

- **Sniper Tower** (extra toren)  
  - Bereik: zeer groot  
  - Schade: hoog  
  - Eigenschap: schiet langzaam maar extreem krachtig.  

---

## 5. Vijanden  
- **Scout**  
  - Snelheid: hoog  
  - Levens: laag  
  - Eigenschap: moeilijk te raken door snelheid.  

- **Trooper**  
  - Snelheid: medium  
  - Levens: medium  
  - Eigenschap: extra weerstand tegen vertragingseffecten.  

- **Heavy**  
  - Snelheid: laag  
  - Levens: hoog.  
  - Eigenschap: kunnen torens aanvallen.  

---

## 6. Gameplay loop  
1. Speler plaatst torens met scrap.  
2. Wave start: vijanden spawnen en lopen richting de basis.  
3. Torens schieten automatisch.  
4. Verslagen vijanden geven scrap terug.  
5. Speler investeert in upgrades en start volgende wave.  

---

## 7. Progressie  
- Sterkere vijanden per wave.  
- Kortere spawn-intervallen.  
- Nieuwe vijandtypes (maybe).  
- Hogere kosten voor torens/upgrades.  

---

## 8. Risico’s en oplossingen volgens PIO  

**Probleem 1:** Scrap-economie niet gebalanceerd.  
- **Impact:** Spel kan te makkelijk of onmogelijk worden.  
- **Oplossing:** Veel testen en aanpassen van kosten en opbrengsten.  

**Probleem 2:** Vijanden blokkeren pad of glitch door torens heen.  
- **Impact:** Bugs die gameplay breken.  
- **Oplossing:** Waypoint-systeem met fallback-checks.  

**Probleem 3:** Te veel vijanden op scherm → performance issues.  
- **Impact:** Laggy gameplay.  
- **Oplossing:** Optimaliseren met object pooling en simpele animaties.  

---

## 9. Planning per sprint en mechanics  

- **Sprint 1:** Vijanden bewegen over pad, basisverlies werkt.  
- **Sprint 2:** Torens plaatsen en automatisch schieten.  
- **Sprint 3:** Waves en spawns toevoegen, energie verzamelen.  
- **Sprint 4:** Upgrades.  
- **Sprint 5:** UI afronden, balans, performance, polish.  

---

## 10. Inspiratie  
**Inspiratie:** *Defense Grid: The Awakening*  
- **Meenemen:** duidelijke futuristische stijl en strategie in torenplaatsing.  
- **Vermijden:** lange downtimes zonder actie.  

---

## 11. Technisch ontwerp mini  

### 11.1 Vijandbeweging over het pad  
- **Keuze:** Vijanden volgen vaste waypoints.  
- **Risico:** Vijanden lopen verkeerd of blijven hangen.  
- **Oplossing:** Afstand-check naar waypoint, daarna volgende.  
- **Acceptatie:** 20 vijanden lopen zonder vastlopers naar de basis.  

### 11.2 Doel kiezen en schieten  
- **Keuze:** Torens richten op de eerste vijand in bereik (first in line).  
- **Risico:** Torens verspillen schoten.  
- **Oplossing:** Doelprioriteit aanpassen (bijv. sterkste of snelste vijand).  
- **Acceptatie:** Torens blijven schieten op logische targets.  

### 11.3 Waves en spawnen  
- **Keuze:** Vijanden spawnen per batch met interval.  
- **Risico:** Spawning tegelijk of verkeerd getimed.  
- **Oplossing:** Coroutine/timer systeem voor spawns.  
- **Acceptatie:** 3 waves starten correct met verschillende vijanden.  

### 11.4 Economie en levens  
- **Keuze:** Scrap = geld. Vijanden geven scrap bij dood, levens dalen bij goal.  
- **Risico:** Te veel of te weinig beloning.  
- **Oplossing:** Waardes balanceren door testen.  
- **Acceptatie:** Speler kan gemiddeld 8–10 waves halen zonder onbalans.  

### 11.5 UI basis  
- **Keuze:** Eenvoudige HUD met scrap, base health, wave teller, start/pauze knoppen.  
- **Risico:** Te druk scherm.  
- **Oplossing:** UI in een vaste balk onderin.  
- **Acceptatie:** Alles blijft leesbaar tijdens gameplay.  

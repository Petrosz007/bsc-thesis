# Időpont foglaló webes alkalmazás
## Folder Structure
```
docs        
 \ formal-requirements  - Követelmények, hivatalos dokumentumok, segédletek
 \ thesis               - A tényleges szakdolgozat
 \ planning             - Tervezési dokumentumok
src
 \ frontend             - Frontend kód
 \ IWA_Backend          - Backend kód
```

## Development
Development should be done on `127.0.0.1` instead of `localhost`, because Chrome doesn't allow cookies to be set from localhost.

## Deployment
To build the docker image, run `docker build -t <image_name> .` in `src\IWA_Backend`

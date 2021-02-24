dotnet test `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=lcov `
    /p:CoverletOutput=./lcov.info `
    '/p:Exclude=\"[*]*DTOs.*,[*]*Entities.*,[*]*Exceptions.*\"' `
    ..\IWA_Backend\IWA_Backend.Tests\
reportgenerator "-reports:..\IWA_Backend\IWA_Backend.Tests\lcov.info" "-targetdir:coveragereport" -reporttypes:Html
Invoke-Item coveragereport\index.htm
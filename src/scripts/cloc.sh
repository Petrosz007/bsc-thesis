cd ../..
cloc $(git ls-files | grep "^src/" | grep -v "/Migrations/" | grep -v ".gitignore" | grep -v ".sln$" | grep -v ".csproj")
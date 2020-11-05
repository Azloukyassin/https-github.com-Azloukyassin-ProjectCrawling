FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
COPY Projektor.WebApp/bin/Release/netcoreapp2.2/publish/ WebApp/

RUN  apt-get update
RUN  apt-get install -y wget
# rm -rf /var/lib/apt/lists/*

# Install Chrome
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN dpkg -i google-chrome-stable_current_amd64.deb; apt-get -fy install
# Remove Chrome installation file
RUN rm google-chrome-stable_current_amd64.deb

WORKDIR /WebApp/

# Install unzipper
RUN apt-get install unzip
# Install dependencies for ChromeDriver
RUN apt-get install -y unzip xvfb libxi6 libgconf-2-4
# Install ChromeDriver
RUN wget https://chromedriver.storage.googleapis.com/77.0.3865.10/chromedriver_linux64.zip
RUN unzip chromedriver_linux64.zip
# Remove ChromeDriver installation file
RUN rm chromedriver_linux64.zip
ENTRYPOINT ["dotnet", "WebApp.dll"]
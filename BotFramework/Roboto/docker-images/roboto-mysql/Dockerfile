FROM mysql:8.0

LABEL Description="MySQL Database for Roboto Calendar Scheduler Service"

ENV MYSQL_DATABASE=RobotoCalendarScheduler
ENV MYSQL_USER=roboto

COPY ./SetupDatabase.sql /docker-entrypoint-initdb.d/01-setup.sql

EXPOSE 3306
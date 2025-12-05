# roboto-mysql MySQL Docker Image

The purpose of this container is to serve as database for Roboto.Service RESTful API.

## How to setup container correctly

### Building image

Locate Dockerfile and run the following command on terminal (Tag name is optional but highly recommended)

```bash
docker build -t roboto/mysql-db .
```

### How to run container

### Database Setup

Update login password in `SetupDatabase.sql` to a new password. Take in mind that some special characters my not work as expected.

```sql
-- Create user
CREATE USER IF NOT EXISTS 'roboto'@'%' IDENTIFIED BY 'change_this_password';
```

### Build Image

Run the following command on the terminal (container name is optional but highly recommended).

**Note**: Use password previosuly set on `SetupDatabase.sql`

```bash
docker run -d -p 3306:3306 -e MYSQL_ROOT_PASSWORD={yourPasswordHere} -e MYSQL_PASSWORD={yourPasswordHere} --name roboto-mysql roboto/mysql-db
```

### How to connect to database

Either use MySql Workbench, MySql Visual Studio Code extension or Sqlectron to connect to database using username **roboto@localhost** and the password defined on `SetupDatabase.sql`.

### Removing Container and Volumes

MySQL may keep configuration after first creation. This includes user login and roles. Make sure to force removal of container and volumes in order to start a fresh new container and volumes by the following command on terminal:

```bash
docker rm -f roboto-mysql
docker volume prune -f
```

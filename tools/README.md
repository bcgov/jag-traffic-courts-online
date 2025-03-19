# Development Environment

Running TCO locally involves a lot of services and requires a lot of configuration settings. These settings include 
username and passwords. These credentials cannot be checked into source control without encryption.

## Reqirements 

### [age](https://github.com/FiloSottile/age)  

Age is used to encrypt the environment variables file for local developent enviroments.

1. Install the `age` secure file encryption tool.
2. Create a `age` key and add an environment variable AGE_KEY_FILE pointing to this file. Where you store this key is up to you. These instructions store the key in the `.age` directory in your user profile.

    ```cmd
    CD %USERPROFILE%
    MKDIR .age
    CD .age
    age-keygen -o key.txt
    setx AGE_KEY_FILE %USERPROFILE%\.age\key.txt
    ```

3. Backup this `age` key file somewhere secure.
4. Submit a pull request to add your public key to the `age-recipients.txt` file. The `age-recipients.txt` is used to encrypt the environment variable file using the specified keys.
5. Once the PR is merged, someone else on the team can reencrypt the file with your key.

## Decrypting the Environment Variable File

Run the `decrypt-env.cmd` file to decrypt the `env.age` which will place the `.env` in the project root directory for use with docker compose.  

## Encrypting the Environment Variable File

If you need to modify the environment file or add recipients, the `encrypt-env.cmd` can be executed. This script will read the `.env` file from the root directory using the `age-recipients.txt` file.

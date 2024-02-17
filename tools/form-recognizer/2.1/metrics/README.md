# Form Recognizer OCR Metrics

This project stores and compares a collection of Violation Ticket images for metric purposes.

The below director is used in the project:

```bash
.
├── data
│   └── db  
│       └── ocr.mv.db - an H2 database holding the results of these images.
└── images
    ├── generated - contains auto-generated images
    └── original  - contains images from a local police department
```

The images folder contains a paired collection of an image and a corresponding sql file of human-vetted values.

## H2 database
The database used in the project is an H2 database in standalone mode. To launch the database and to connect it to the above .\data\db\ocr.mv.db file, from the parent \tools folder, run:

```bash
docker-compose up -d h2
```

## Reseed the database
To reprocess any of the images, visit the Swagger UI (`http://localhost:8088/swagger-ui/index.html`)
Invoke the single endpoint `GET /processImages` to reprocess all the images in the images folder.

To reseed the human-vetted images, use the h2-console or your favourite sql client to clear out the database and run all the SQL scripts in the images folder.
  
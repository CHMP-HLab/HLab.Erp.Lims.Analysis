UPDATE "SAMPLE" SET "AnalysisMotivationId" = 1 WHERE "Motivation" = 'Routine';
UPDATE "SAMPLE" SET "AnalysisMotivationId" = 3 WHERE "Motivation" = 'Enregistrement';

DELETE FROM "SampleForm" WHERE "ResultValues" IS NULL;

DELETE FROM
    "SampleForm" a
        USING "SampleForm" b
WHERE
    a."Id" < b."Id"
    AND a."FormClassId" = b."FormClassId" AND a."SampleId" = b."SampleId";

ALTER TABLE public."SampleForm"
    ADD UNIQUE ("SampleId", "FormClassId");
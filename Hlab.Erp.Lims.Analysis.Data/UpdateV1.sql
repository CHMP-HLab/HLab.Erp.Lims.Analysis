ALTER TABLE public."SampleTest"
    ADD COLUMN "SpecificationsDone" boolean;

ALTER TABLE public."SampleTest"
    ADD COLUMN "ScheduledDate" timestamp without time zone;

ALTER TABLE public."SampleTestResult"
    ADD COLUMN "MandatoryDone" boolean;

ALTER TABLE public."SampleTestResult"
    ADD COLUMN "Note" text;

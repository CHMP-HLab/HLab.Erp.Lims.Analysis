ALTER TABLE public."SampleTest"
    ADD COLUMN "SpecificationsDone" boolean;

ALTER TABLE public."SampleTest"
    ADD COLUMN "ScheduledDate" timestamp without time zone;

ALTER TABLE public."SampleTestResult"
    ADD COLUMN "MandatoryDone" boolean;

ALTER TABLE public."SampleTestResult"
    ADD COLUMN "Note" text;

-- SEQUENCE: public."ProductCategory_Id_seq"

-- DROP SEQUENCE public."ProductCategory_Id_seq";

CREATE SEQUENCE public."ProductCategory_Id_seq"
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public."ProductCategory_Id_seq"
    OWNER TO postgres;

GRANT SELECT, USAGE ON SEQUENCE public."ProductCategory_Id_seq" TO lims;

GRANT ALL ON SEQUENCE public."ProductCategory_Id_seq" TO postgres;

-- Table: public."ProductCategory"

-- DROP TABLE public."ProductCategory";

CREATE TABLE public."ProductCategory"
(
    "Id" integer NOT NULL DEFAULT nextval('"ProductCategory_Id_seq"'::regclass),
    "Name" text COLLATE pg_catalog."default",
    "Priority" integer,
    "IconPath" text COLLATE pg_catalog."default",
    CONSTRAINT productcategory_pkey PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE public."ProductCategory"
    OWNER to postgres;

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public."ProductCategory" TO lims;

GRANT ALL ON TABLE public."ProductCategory" TO postgres;

ALTER TABLE public."Product"
    ADD COLUMN "CategoryId" integer;
ALTER TABLE public."Product"
    ADD FOREIGN KEY ("CategoryId")
    REFERENCES public."ProductCategory" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;

INSERT INTO public."ProductCategory" (
"Id", "Name", "IconPath") VALUES (
'1'::integer, 'Médicament'::text, 'Icons/Sample/Drugs'::text)
 returning "Id";

INSERT INTO public."ProductCategory" (
"Id", "Name", "IconPath") VALUES (
'2'::integer, 'Consommable Médical'::text, 'Icons/Sample/Consummables'::text)
 returning "Id";

update "Product" Set "CategoryId"=1 WHERE "CategoryId" ISNULL;

ALTER TABLE public."Product"
    ADD COLUMN "Complement" text;

ALTER TABLE public."SampleTestResult"
    ADD COLUMN "Conformity" text;

ALTER TABLE public."User"
    ADD COLUMN "Domain" text;	
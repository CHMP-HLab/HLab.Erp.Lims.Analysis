DELETE FROM public."SampleForm" WHERE "SampleId" < 0;

UPDATE public."Pharmacopoeia" SET
"Name" = '{International Pharmacopeia}'::text WHERE
"Id" = 1;
UPDATE public."Pharmacopoeia" SET
"Name" = '{European Pharmacopoeia}'::text WHERE
"Id" = 2;
UPDATE public."Pharmacopoeia" SET
"Name" = '{United States Pharmacopeia}'::text WHERE
"Id" = 3;
UPDATE public."Pharmacopoeia" SET
"Name" = '{British Pharmacopoeia}'::text WHERE
"Id" = 4;
UPDATE public."Pharmacopoeia" SET
"Name" = '{Internal Method}'::text WHERE
"Id" = 5;
INSERT INTO public."Pharmacopoeia" (
"Name", "Id") VALUES (
'{Manufaturer Method}'::text, '6'::integer)
 returning "Id";

UPDATE "Icon" SET "Foreground" = -16777216;
UPDATE "Icon" SET "Foreground" = null WHERE "Path" Contains "Flag";


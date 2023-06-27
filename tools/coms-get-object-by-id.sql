
-- get object by notice-of-dispute-id
select o.*, m."key", m."value"
  from "object" as o
 inner join "version" as v on v."objectId" = o."id"
 inner join "version_metadata" as vm on vm."versionId" = v."id"
 inner join "metadata" as m on m."id" = vm."metadataId"
 where m."key" = 'notice-of-dispute-id'
   and m."value" = '08db4680-c1cd-f79e-f4ee-08c4740d0000'

-- get metadata by notice-of-dispute-id
select *
  from "metadata"
 where key = 'notice-of-dispute-id'
 --  and value = '08db4680-c1cd-f79e-f4ee-08c4740d0000'
 order by value desc


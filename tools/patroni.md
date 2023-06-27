
On April 27th, the dev environment ran out of disk space for Postgres. I expanded the volume from 5GB to 6GB to allow
the pod to start. Accessing the pod's shell, I was able to determine the space was being consumed in the pg_wal
directory. The pg_wal logs were not being cleaned up. Based on a comment in this [issue](https://github.com/zalando/postgres-operator/issues/2012),
I was able 



postgres@postgres-0:~$ du -hs pgdata/pgroot/data/pg_wal
4.7G    pgdata/pgroot/data/pg_wal

postgres@postgres-0:~$ psql
psql (15.2 (Ubuntu 15.2-1.pgdg22.04+1))
Type "help" for help.

postgres=# SELECT * FROM pg_replication_slots;
 slot_name  | plugin | slot_type | datoid | database | temporary | active | active_pid | xmin | catalog_xmin | restart_lsn | confirmed_flush_lsn | wal_status | safe_wal_size | two_phase
------------+--------+-----------+--------+----------+-----------+--------+------------+------+--------------+-------------+---------------------+------------+---------------+-----------
 postgres_1 |        | physical  |        |          | f         | t      |       1413 |      |              | 1/6D000060  |                     | reserved   |               | f
 postgres_2 |        | physical  |        |          | f         | t      |       1455 |      |              | 1/6D000060  |                     | reserved   |               | f
(2 rows)


postgres@postgres-0:~$ patronictl list
+ Cluster: postgres --------+---------+---------+----+-----------+
| Member     | Host         | Role    | State   | TL | Lag in MB |
+------------+--------------+---------+---------+----+-----------+
| postgres-0 | 10.97.112.30 | Leader  | running | 20 |           |
| postgres-1 | 10.97.123.72 | Replica | running |  1 |      4770 |
| postgres-2 | 10.97.89.254 | Replica | running |  1 |      4770 |
+------------+--------------+---------+---------+----+-----------+


After 

postgres@postgres-0:~$ patronictl list
+ Cluster: postgres --------+---------+---------+----+-----------+
| Member     | Host         | Role    | State   | TL | Lag in MB |
+------------+--------------+---------+---------+----+-----------+
| postgres-0 | 10.97.112.30 | Leader  | running | 20 |           |
| postgres-1 | 10.97.123.72 | Replica | running | 20 |         0 |
| postgres-2 | 10.97.89.254 | Replica | running | 20 |         0 |
+------------+--------------+---------+---------+----+-----------+
postgres@postgres-0:~$ ls -l pgdata/pgroot/data/pg_wal | wc -l
36
postgres@postgres-0:~$ du -hs pgdata/pgroot/data/pg_wal
225M    pgdata/pgroot/data/pg_wal
postgres@postgres-0:~$
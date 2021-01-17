# 資料庫:[資料表](#table)與[資料](#data)

### -- 資料表<a name="table"></a>
```sql
DROP TABLE IF EXISTS "public"."play_record";
DROP TABLE IF EXISTS "public"."course";
DROP TABLE IF EXISTS "public"."topic";
DROP TABLE IF EXISTS "public"."class_member";
DROP TABLE IF EXISTS "public"."class";
DROP TABLE IF EXISTS "public"."user";


-- user

DROP SEQUENCE IF EXISTS "public"."user_serno_seq";
CREATE SEQUENCE "public"."user_serno_seq" START 1000000;

CREATE TABLE "public"."user" (
	"email" varchar(50) COLLATE "pg_catalog"."default",
	"user_name" varchar(50) COLLATE "pg_catalog"."default"
)
;

ALTER TABLE "public"."user" ADD CONSTRAINT "user_pkey" PRIMARY KEY ("email");

-- class

CREATE TABLE "public"."class" (
	"class_id" char(6) COLLATE "pg_catalog"."default" NOT NULL,
	"school" varchar(50) COLLATE "pg_catalog"."default",
	"name" varchar(50) COLLATE "pg_catalog"."default",
	"email" varchar(50) COLLATE "pg_catalog"."default",
	"max_number" int,
	"topics" varchar(50) COLLATE "pg_catalog"."default"
)
;

ALTER TABLE "public"."class" ADD CONSTRAINT "class_pkey" PRIMARY KEY ("class_id");
ALTER TABLE "public"."class" ADD CONSTRAINT "fk1" FOREIGN KEY("email") REFERENCES "public"."user" ("email") ON DELETE CASCADE ON UPDATE CASCADE;

-- class_member

DROP SEQUENCE IF EXISTS "public"."class_member_serno_seq";
CREATE SEQUENCE "public"."class_member_serno_seq" START 1000000;

CREATE TABLE "public"."class_member" (
	"member_id" char(8) COLLATE "pg_catalog"."default" NOT NULL DEFAULT ('M'::text || (nextval('class_member_serno_seq'::regclass))::text),
	"class_id" char(6) COLLATE "pg_catalog"."default" NOT NULL,
	"member_name" varchar(50) COLLATE "pg_catalog"."default",
	"pin" char(4) COLLATE "pg_catalog"."default" NOT NULL,
	"last_played" timestamptz DEFAULT now()
)
;

ALTER TABLE "public"."class_member" ADD CONSTRAINT "class_member_pkey" PRIMARY KEY ("member_id");
ALTER TABLE "public"."class_member" ADD CONSTRAINT "fk1" FOREIGN KEY("class_id") REFERENCES "public"."class" ("class_id") ON DELETE CASCADE ON UPDATE CASCADE;

-- topic --

DROP SEQUENCE IF EXISTS "public"."topic_serno_seq";
CREATE SEQUENCE "public"."topic_serno_seq" START 100;

CREATE TABLE "public"."topic" (
	"topic_id" char(4) COLLATE "pg_catalog"."default" NOT NULL DEFAULT ('T'::text || (nextval('topic_serno_seq'::regclass))::text),
	"topic_name" varchar(50) COLLATE "pg_catalog"."default" NOT NULL,
	"topic_color" char(6) COLLATE "pg_catalog"."default"
)
;

ALTER TABLE "public"."topic" ADD CONSTRAINT "topic_pkey" PRIMARY KEY ("topic_id");

-- course

DROP SEQUENCE IF EXISTS "public"."course_serno_seq";
CREATE SEQUENCE "public"."course_serno_seq" START 1000000;

CREATE TABLE "public"."course" (
  	"course_id" char(8) COLLATE "pg_catalog"."default" NOT NULL DEFAULT ('C'::text || (nextval('course_serno_seq'::regclass))::text),
	"course_name" varchar(50) COLLATE "pg_catalog"."default",
	"description" varchar(256) COLLATE "pg_catalog"."default",
	"topic_id" char(4) COLLATE "pg_catalog"."default" NOT NULL,
	"course_json" varchar(1024) COLLATE "pg_catalog"."default"
)
;

ALTER TABLE "public"."course" ADD CONSTRAINT "course_pkey" PRIMARY KEY ("course_id");
ALTER TABLE "public"."course" ADD CONSTRAINT "fk1" FOREIGN KEY("topic_id") REFERENCES "public"."topic" ("topic_id") ON DELETE RESTRICT ON UPDATE RESTRICT;

-- play_record

DROP SEQUENCE IF EXISTS "public"."play_record_serno_seq";

CREATE TABLE "public"."play_record" (
	"member_id" char(8) COLLATE "pg_catalog"."default" NOT NULL,
	"course_id" char(8) COLLATE "pg_catalog"."default" NOT NULL,
	"score_time" int DEFAULT 0,
	"score_amount" int DEFAULT 0,
	"score_blocks" int DEFAULT 0,
	"board" varchar(256) COLLATE "pg_catalog"."default"
)
;

ALTER TABLE "public"."play_record" ADD CONSTRAINT "play_record_pkey" PRIMARY KEY ("member_id", "course_id");
ALTER TABLE "public"."play_record" ADD CONSTRAINT "fk1" FOREIGN KEY("member_id") REFERENCES "public"."class_member" ("member_id") ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE "public"."play_record" ADD CONSTRAINT "fk2" FOREIGN KEY("course_id") REFERENCES "public"."course" ("course_id") ON DELETE CASCADE ON UPDATE CASCADE;
```

### -- 資料<a name="data"></a>
```sql
INSERT INTO public.topic (topic_name,topic_color) VALUES 
	('基礎','D62F2F')
	,('機關','337CA0')
	,('迴圈','FF551C')
	,('條件式','33673B')
	;

INSERT INTO public.course (course_name,description,topic_id,course_json) VALUES 
	('lv1','基礎1','T100','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":4
		},
		"gameEnv":"000000000000000200040000000000000000000000",
		"playerDir":1
	}')
	,('lv2','基礎2','T100','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":5,
			"TurnBlock":1
		},
		"gameEnv":"000000000000400000000000000000020000000000",
		"playerDir":0
	}')
	,('lv3','基礎3','T100','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":12,
			"TurnBlock":4
		},
		"gameEnv":"000000000010000201040000100000010000000000",
		"playerDir":1
	}')
	,('lv4','基礎4','T100','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":6,
			"TurnBlock":1
		},
		"gameEnv":"001011110130040010111101010000101000012100",
		"playerDir":0
	}')
	,('lv5','基礎5','T100','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":15,
			"TurnBlock":6
		},
		"gameEnv":"010401001000100133310010001001030100112110",
		"playerDir":0
	}')
	,('lv1','機關1','T101','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":12,
			"TurnBlock":10
		},
		"gameEnv":"001111011160111470302111001100161100011100",
		"playerDir":3
	}')
	,('lv2','機關2','T101','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":9,
			"TurnBlock":5
		},
		"gameEnv":"111111100050012005041000000111111111111111",
		"playerDir":1
	}')
	,('lv3','機關3','T101','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":10,
			"TurnBlock":6
		},
		"gameEnv":"111111110040011050051103003115055011002001",
		"playerDir":0
	}')
	,('lv4','機關4','T101','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":12,
			"TurnBlock":0
		},
		"gameEnv":"000000102000300055554005555000000300000001",
		"playerDir":1
	}')
	,('lv5','機關5','T101','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":7
		},
		"gameEnv":"555555554300055333335500323550033355555555",
		"playerDir":3
	}')
	,('lv1','迴圈1','T102','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":1,
			"RepeatBlock:0":1
		},
		"gameEnv":"000000000000001111111200000411111110000000",
		"playerDir":1
	}')
	,('lv2','迴圈2','T102','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":2,
			"TurnBlock":2,
			"RepeatBlock:0":1
		},
		"gameEnv":"251111100511115005111150051111500511115041",
	}')
	,('lv3','迴圈3','T102','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":12,
			"TurnBlock":8
			"RepeatBlock":4
		},
		"gameEnv":"200000055555300000000035555500000005555504",
		"playerDir":1
	}')
	,('lv4','迴圈4','T102','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock:0":10,
			"MoveBlock:2":6,
			"TurnBlock":4,
			"RepeatBlock:0":1
		},
		"gameEnv":"656565505050553535355000007401111112111111",
		"playerDir":0
	}')
	,('lv5','迴圈5','T102','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock:0":12,
			"MoveBlock:2":6,
			"RepeatBlock":2
		},
		"gameEnv":"001610000131001110111630270411131110016100",
		"playerDir":1
	}')
	,('lv1','條件式1','T103','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":1,
			"TurnBlock":1,
			"RepeatBlock":1,
			"IfBlock":1
		},
		"gameEnv":"555555555555455555505555550555555052000005",
		"playerDir":0
	}')
	,('lv2','條件式2','T103','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":1,
			"TurnBlock":1,
			"RepeatBlock":1,
			"IfBlock":1
		},
		"gameEnv":"555555555000055504505555550552000055555555",
		"playerDir":1
	}')
	,('lv3','條件式3','T103','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":10,
			"TurnBlock":4,
			"RepeatBlock:0":1,
			"IfBlock":1
		},
		"gameEnv":"655665505500553553355000007405555552555555",
		"playerDir":0
	}')
	,('lv4','條件式4','T103','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock":5,
			"TurnBlock":10,
			"RepeatBlock:0":1,
			"IfBlock":1
		},
		"gameEnv":"155455115000511005001105550110050011002001",
		"playerDir":0
	}')
	,('lv5','條件式5','T103','{
		"blocksList":{
			"StartBlock":1,
			"MoveBlock:1":10,
			"MoveBlock:2":10
			"TurnBlock":8,
			"RepeatBlock:0":1,
			"IfBlock":1
		},
		"gameEnv":"655651135535112000074533531156656111111111",
		"playerDir":1
	}')
	;
```
--
-- PostgreSQL database dump
--

-- Dumped from database version 14.1
-- Dumped by pg_dump version 14.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: battles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.battles (
    id character varying(128) NOT NULL,
    user1 character varying(128),
    user2 character varying(128),
    log text,
    winner character varying(128)
);


ALTER TABLE public.battles OWNER TO mtcg;

--
-- Name: cards; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cards (
    id character varying(128) NOT NULL,
    name character varying(128),
    damage double precision,
    owner character varying(128)
);


ALTER TABLE public.cards OWNER TO mtcg;

--
-- Name: deck; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.deck (
    owner character varying(128) NOT NULL,
    deck text
);


ALTER TABLE public.deck OWNER TO mtcg;

--
-- Name: packages; Type: TABLE; Schema: public; Owner: mtcg
--

CREATE TABLE public.packages (
    boughtby character varying(128),
    data text,
    id character varying(128) NOT NULL
);


ALTER TABLE public.packages OWNER TO mtcg;

--
-- Name: scoreboard; Type: TABLE; Schema: public; Owner: mtcg
--

CREATE TABLE public.scoreboard (
    username character varying(128) NOT NULL,
    rating integer
);


ALTER TABLE public.scoreboard OWNER TO mtcg;

--
-- Name: users; Type: TABLE; Schema: public; Owner: mtcg
--

CREATE TABLE public.users (
    username character varying(128) NOT NULL,
    password character varying(256),
    balance double precision DEFAULT 20,
    name character varying(128) DEFAULT ''::character varying,
    bio character varying(512) DEFAULT ''::character varying,
    image character varying(256) DEFAULT ''::character varying
);


ALTER TABLE public.users OWNER TO mtcg;

--
-- Name: battles battles_pkey; Type: CONSTRAINT; Schema: public; Owner: mtcg
--

ALTER TABLE ONLY public.battles
    ADD CONSTRAINT battles_pkey PRIMARY KEY (id);


--
-- Name: cards cards_pkey; Type: CONSTRAINT; Schema: public; Owner: mtcg
--

ALTER TABLE ONLY public.cards
    ADD CONSTRAINT cards_pkey PRIMARY KEY (id);


--
-- Name: deck deck_pkey; Type: CONSTRAINT; Schema: public; Owner: mtcg
--

ALTER TABLE ONLY public.deck
    ADD CONSTRAINT deck_pkey PRIMARY KEY (owner);


--
-- Name: packages packages_pkey; Type: CONSTRAINT; Schema: public; Owner: mtcg
--

ALTER TABLE ONLY public.packages
    ADD CONSTRAINT packages_pkey PRIMARY KEY (id);


--
-- Name: scoreboard scoreboard_pkey; Type: CONSTRAINT; Schema: public; Owner: mtcg
--

ALTER TABLE ONLY public.scoreboard
    ADD CONSTRAINT scoreboard_pkey PRIMARY KEY (username);


--
-- mtcgQL database dump complete
--


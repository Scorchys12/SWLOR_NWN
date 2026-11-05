# Echani temple monks draft

Master Vaelis, Disciple Ren and Disciple Sai use their own tags and controller. Their hooded vest/coat pieces are catalog-based approximations of the supplied reference. Color indices are provisional until rendered in the client.

Temple courtyard placement is inferred from the supplied screenshot and the stone-pattern floor/kiosk cluster at (110, 21) in Silver Gate; it needs client confirmation.

Route: Silver Gate temple, Old Quarter residents, Tunnels, Riverway, Veylan Groves, Farmlands/housing, return through Silver Gate, Highcrest, Verdant Crown, Hearth Ward/starport, Proving Grounds, Keshan Heights, Keshan Peaks, Spa Resort, then return. Existing transition endpoints supply transfer destinations. Cross-area transfers occur only after all three reach the authored source stop. Paths, elevation changes, door clearance and same-area shuttle access need live verification.

Map-note audit covers Eshan town areas and the route to the spa. Added identifiable guilds, market, flower shop, named residences, residential district, temple courtyard, cantina sign, spa dining area and guest rooms. Existing starport flight-terminal note retained. Scenic houses and unnamed tables are not asserted to be usable houses or restaurants. No existing NPC or patrol data is changed.

Manual validation: the owner will check outfits, temple placement, public walking approaches, and the full live route and conversations in the NWN client.

Packing correction: removed invalid 17-byte extended armor labels from the three outfit blueprints and their placed copies, and synchronized the native extended bicep fields with the intended byte values. A regression test checks native label lengths and matching armor parts.

Initial draft validation: server/test-project and CLI builds succeeded. 44 unique focused tests passed (EchaniTempleMonksTests, CivilianPatrolTests, EshanOldQuarterCivilianTests, and the module/C# text-encoding checks). Content audit confirmed 3 new placed NPCs, 84 route waypoints, 21 map notes, one closed itinerary, 33 valid transfer destinations, and preservation of all 7,875 existing area objects. All 24 changed module resources converted to native GFF. The full module packed successfully; extraction and round-trip checks matched the 108 added objects and all new blueprints against the source. These checks do not establish rendered appearance, placement clearance, or live movement.

Eshan integration: merged the current Eshan-fixes branch and retained all 7,915 existing objects in its 15 affected areas. Only the three monks, 84 route waypoints, and 21 map notes were inserted into those area files; existing base objects and non-list fields are unchanged. The two Highcrest shuttle steps now use normal walking because the base branch retired those same-area shuttle endpoints, leaving 31 transfer links. The merged build and 44 unique focused test cases passed after this correction.

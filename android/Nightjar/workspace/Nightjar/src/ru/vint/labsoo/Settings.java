package ru.vint.labsoo;

import android.os.Bundle;
import android.preference.PreferenceActivity;
import ru.vint.nightjar.R;

public class Settings extends PreferenceActivity 
{
	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
	    super.onCreate(savedInstanceState);
	    addPreferencesFromResource(R.xml.settings);
	}
}
